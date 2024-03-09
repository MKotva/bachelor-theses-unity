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
            lastActionRecord = null;
            lastActionRecordReverse = null;

            _isMouseDown = true;
            if (map.Selected.Count != 0)
            {
                var positions = GetSelectedPositionsString();
                lastActionRecordReverse = new JournalActionDTO($"SS;{positions}", PerformAction);
                map.UnSelectAll();
            }
            else
            {
                lastActionRecordReverse = new JournalActionDTO("SUA", PerformAction);
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
                lastActionRecord = new JournalActionDTO($"SG;{mousePosition.x}:{mousePosition.y}", PerformAction);
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
                map.UnSelectAll();
                SelectAllGroupItems(MathHelper.GetVector3FromString(descriptions[1]));
            }
            else if(descriptions[0] == "SUA")
            {
                map.UnSelectAll();
            }
        }

        private void SelectAllGroupItems(Vector3 position)
        {
            var worldCellPosition = map.GetCellCenterPosition(position);
            GameObject objectAtPos = map.GetObjectAtPosition(worldCellPosition);

            if (objectAtPos == null)
                return;

            foreach (var key in map.Data.Keys)
            {
                if (map.Data[key].ContainsKey(worldCellPosition))
                {
                    foreach (var item in map.Data[key])
                    {
                        map.Marker.MarkObject(item.Value);
                        map.Selected.Add(item.Key, (item.Value, false));
                    }
                }
            }
        }
        private string GetSelectedPositionsString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var selectedPos in map.Selected.Keys)
            {
                sb.Append($"{selectedPos.x}:{selectedPos.y};");
            }
            return sb.ToString();
        }
    }
}

