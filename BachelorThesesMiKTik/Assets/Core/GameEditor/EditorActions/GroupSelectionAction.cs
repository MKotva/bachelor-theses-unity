using Assets.Core.GameEditor.DTOS;
using Assets.Core.GameEditor.DTOS.EditorActions;
using System.Collections.Generic;
using System.Linq;
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
                lastActionRecordReverse = new PositionOperation(selectAction.SingleSelection, map.Selected.Keys.ToList());
                map.UnselectAll();
            }
            else
            {
                lastActionRecordReverse = new JournalActionDTO(selectAction.UnselectAll);
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
                lastActionRecord = new PositionOperation(Perform , new List<Vector3>() { mousePosition });
                SaveRecord(lastActionRecord, lastActionRecordReverse);
            }
            isMouseDown = false;
        }

        /// <summary>
        /// This method will perform action based on given JournalAction.
        /// </summary>
        /// <param name="action"></param>
        public void Perform(JournalActionDTO action)
        {
            if(action is PositionOperation)
            {
                var positionAction = (PositionOperation)action;
                if(positionAction.Positions.Count > 0) 
                {
                    map.UnselectAll();
                    SelectAllGroupItems(positionAction.Positions[0]);   
                }
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
    }
}

