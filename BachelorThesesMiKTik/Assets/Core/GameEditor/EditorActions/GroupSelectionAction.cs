using Assets.Core.GameEditor;
using Assets.Core.GameEditor.DTOS;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Scenes.GameEditor.Core.EditorActions
{
    public class GroupSelectionAction : EditorActionBase
    {
        bool isMouseDown;
        SelectAction selectAction;

        public GroupSelectionAction()
        {
            selectAction = new SelectAction();
        }

        /// <summary>
        ///  On left mouse key press, this method will allow performing GroupSelect action.
        ///  If there is any selected item, 
        /// </summary>
        /// <param name="key"></param>
        public override void OnMouseDown(MouseButton key) 
        {
            lastActionRecord = null;
            lastActionRecordReverse = null;

            isMouseDown = true;
            if (map.Selected.Count != 0)
            {
                var positions = GetSelectedPositionsString();
                lastActionRecordReverse = new JournalActionDTO($"SS;{positions}", selectAction.PerformAction);
                map.UnselectAll();
            }
            else
            {
                lastActionRecordReverse = new JournalActionDTO("SUA", selectAction.PerformAction);
            }
        }

        public override void OnMouseUp() 
        {
            isMouseDown = false;
        }

        public override void OnUpdate(Vector3 mousePosition) 
        {
            if(isMouseDown)
            {
                SelectAllGroupItems(mousePosition);
                lastActionRecord = new JournalActionDTO($"SG;{mousePosition.x}:{mousePosition.y}", PerformAction);
            }
            isMouseDown = false;
        }

        /// <summary>
        /// This method will perform action based on given string.
        /// </summary>
        /// <param name="action"></param>
        public override void PerformAction(string action)
        {
            var descriptions = action.Split(';');
            if (descriptions.Length < 1)
            {
                return;
            }
            if (descriptions[0] == "SG")
            {
                map.UnselectAll();
                SelectAllGroupItems(MathHelper.GetVector3FromString(descriptions[1]));
            }
        }

        /// <summary>
        /// Finds object on given position (if exists) and selects all items
        /// with same ItemID (same type of objects)
        /// </summary>
        /// <param name="position">Mouse click position.</param>
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

        /// <summary>
        /// Goes thru all selected objects and returs theirs positions in one string.
        /// </summary>
        /// <returns></returns>
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

