using Assets.Core.GameEditor;
using Assets.Core.GameEditor.DTOS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace Assets.Scenes.GameEditor.Core.EditorActions
{
    public class GroupSelectionAction : EditorActionBase
    {
        bool _isMouseDown;

        public GroupSelectionAction(MapCanvasController context) : base(context) {}
        public override void OnMouseDown(MouseButton key) 
        {
            _lastActionRecord = null;
            _lastActionRecordReverse = null;

            _isMouseDown = true;
            if (context.Selected.Count != 0)
            {
                var positions = GetSelectedPositionsString();
                _lastActionRecordReverse = new JournalActionDTO($"SS;{positions}", PerformAction);
                context.UnSelectAll();
            }
            else
            {
                _lastActionRecordReverse = new JournalActionDTO("SUA", PerformAction);
            }
        }

        public override void OnMouseUp() 
        {
            _isMouseDown = false;
        }

        public override void OnUpdate(Vector3 mousePosition) 
        {
            if(_isMouseDown)
            {
                SelectAllGroupItems(mousePosition);
                _lastActionRecord = new JournalActionDTO($"SG;{mousePosition.x}:{mousePosition.y}", PerformAction);
            }
            _isMouseDown = false;
        }

        public override void PerformAction(string action)
        {
            var descriptions = action.Split(';');
            if (descriptions.Length < 1)
            {
                return;
            }
            if (descriptions[0] == "SG")
            {
                context.UnSelectAll();
                SelectAllGroupItems(MathHelper.GetVector3FromString(descriptions[1]));
            }
            else if(descriptions[0] == "SUA")
            {
                context.UnSelectAll();
            }
        }

        private void SelectAllGroupItems(Vector3 position)
        {
            var worldCellPosition = context.GetCellCenterPosition(position);
            GameObject objectAtPos = context.GetObjectAtPosition(worldCellPosition);

            if (objectAtPos == null)
                return;

            foreach (var key in context.Data.Keys)
            {
                if (context.Data[key].ContainsKey(worldCellPosition))
                {
                    foreach (var item in context.Data[key])
                    {
                        context.MarkObject(item.Value);
                        context.Selected.Add(item.Key, (item.Value, false));
                    }
                }
            }
        }
        private string GetSelectedPositionsString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var selectedPos in context.Selected.Keys)
            {
                sb.Append($"{selectedPos.x}:{selectedPos.y};");
            }
            return sb.ToString();
        }
    }
}

