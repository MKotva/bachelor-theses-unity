using Assets.Core.GameEditor;
using Assets.Core.GameEditor.DTOS;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Scenes.GameEditor.Core.EditorActions
{
    public class GroupSelectionAction : EditorActionBase
    {
        bool _isMouseDown;
        public override void OnMouseDown(MouseButton key) 
        {
            _lastActionRecord = null;
            _lastActionRecordReverse = null;

            _isMouseDown = true;
            if (editor.Selected.Count != 0)
            {
                var positions = GetSelectedPositionsString();
                _lastActionRecordReverse = new JournalActionDTO($"SS;{positions}", PerformAction);
                editor.UnSelectAll();
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
                editor.UnSelectAll();
                SelectAllGroupItems(MathHelper.GetVector3FromString(descriptions[1]));
            }
            else if(descriptions[0] == "SUA")
            {
                editor.UnSelectAll();
            }
        }

        private void SelectAllGroupItems(Vector3 position)
        {
            var worldCellPosition = editor.GetCellCenterPosition(position);
            GameObject objectAtPos = editor.GetObjectAtPosition(worldCellPosition);

            if (objectAtPos == null)
                return;

            foreach (var key in editor.Data.Keys)
            {
                if (editor.Data[key].ContainsKey(worldCellPosition))
                {
                    foreach (var item in editor.Data[key])
                    {
                        editor.MarkObject(item.Value);
                        editor.Selected.Add(item.Key, (item.Value, false));
                    }
                }
            }
        }
        private string GetSelectedPositionsString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var selectedPos in editor.Selected.Keys)
            {
                sb.Append($"{selectedPos.x}:{selectedPos.y};");
            }
            return sb.ToString();
        }
    }
}

