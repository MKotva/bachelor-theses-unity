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
            _isMouseDown = true;

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
            }
            _isMouseDown = false;
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

        public void UnSelectAllGroupItems()
        {
            foreach (var selected in context.Selected)
            {
                context.UnMarkObject(selected.Value.Item1);
                context.selectedId = -1;
            }
        }
    }
}

