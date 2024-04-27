using Assets.Core.GameEditor;
using Assets.Core.GameEditor.DTOS;
using Assets.Core.GameEditor.DTOS.EditorActions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Scenes.GameEditor.Core.EditorActions
{
    internal class RemoveAction : EditorActionBase
    {
        private bool _isMouseDown;
        
        private bool _selection;
        private Vector2 _position;

        private Dictionary<Vector3, int> _newObjectPostions;
        private InsertAction _insertAction;

        public RemoveAction()
        {
            _newObjectPostions = new Dictionary<Vector3, int>();
            _insertAction = new InsertAction( true); 
        }
        public RemoveAction(bool dummy) 
        {
            _newObjectPostions = new Dictionary<Vector3, int>();
        }

        /// <summary>
        /// Handles mouse left button click by re-initializing action data. 
        /// actions to journal.
        /// </summary>
        /// <param name="button"></param>

        public override void OnMouseDown(MouseButton key)
        {
            if (key == MouseButton.LeftMouse)
            {
                lastActionRecord = null;
                lastActionRecordReverse = null;

                _isMouseDown = true;
                _selection = false;
                _newObjectPostions = new Dictionary<Vector3, int>();
            }
        }

        /// <summary>
        /// Handles mouse button release by saving performed action data to Journal.
        /// </summary>
        /// <param name="button"></param>
        public override void OnMouseUp()
        {
            if (_isMouseDown || _selection)
            {
                if (_selection)
                {
                    lastActionRecord = new PositionOperation(RemoveSelection, new List<Vector3> { _position});
                    lastActionRecordReverse = new ItemOperation(_insertAction.InsertAsSelected, _newObjectPostions);
                }
                else 
                {
                    lastActionRecord = new PositionOperation(Remove, _newObjectPostions.Keys.ToList());
                    lastActionRecordReverse = new ItemOperation(_insertAction.Insert, _newObjectPostions);
                }

                SaveRecord(lastActionRecord, lastActionRecordReverse);
                _isMouseDown = false;
                _selection = false;
            }
        }

        /// <summary>
        /// Called on Unity Update call, based on given position,
        /// performs remove or selection remove action.
        /// </summary>
        /// <param name="mousePosition">Cursor position.</param>
        public override void OnUpdate(Vector3 mousePosition)
        {
            if (_isMouseDown)
            {
                var position = map.GetCellCenterPosition(mousePosition);
                if (map.Selected.ContainsKey(position))
                {
                    _newObjectPostions = RemoveSelection();
                    _position = position;
                    _selection = true;
                    _isMouseDown = false;
                }
                else
                {
                    if(Remove(position, out var removedID))
                        _newObjectPostions.Add(position, removedID);
                }    
            }
        }

        /// <summary>
        /// Removes all objects stored in given journal action.
        /// </summary>
        /// <param name="action"></param>
        public void Remove(JournalActionDTO action)
        {
            if (action is PositionOperation)
            {
                var operationAction = (PositionOperation)action;
                foreach(var position in operationAction.Positions)
                {
                    Remove(position, out var id);
                }
            }
        }

        /// <summary>
        /// Removes objects in selection based on given journal action.
        /// </summary>
        /// <param name="action"></param>
        public void RemoveSelection(JournalActionDTO action)
        {
            if (action is PositionOperation)
            {
                var positionOperation = (PositionOperation) action;
                var cellCenter = map.GetCellCenterPosition(positionOperation.Positions[0]);
                if (map.Selected.ContainsKey(cellCenter))
                {
                    RemoveSelection();
                }
            }
        }

        /// <summary>
        /// Removes object on given position. If position is empty, nothing happens.
        /// Otherwise returns id of removed object on given position.
        /// </summary>
        /// <param name="position">Cursor position.</param>
        /// <param name="removedID">Id of removed object.</param>
        /// <returns>True if object was removed, otherwise false.</returns>
        private bool Remove(Vector3 position, out int removedID)
        {
            position = map.GetCellCenterPosition(position);
            if (map.ContainsObjectAtPosition(position, out var id))
            {
                map.Erase(map.Data[id][position], position);
                removedID = id;
                return true;
            }

            removedID = 0;
            return false;
        }

        /// <summary>
        /// Removes all objects in selection, and stores data about those objects for
        /// journal action.
        /// </summary>
        /// <returns></returns>
        private Dictionary<Vector3, int> RemoveSelection()
        {
            var removedObjects = new Dictionary<Vector3, int>();

            var keys = map.Selected.Keys.ToArray();
            for (int i = 0; i < map.Selected.Count(); i++)
            {
                var position = keys[i];
                if (!map.Selected[position].Item2)
                {
                    if (map.TryGetID(position, out var id))
                        removedObjects.Add(position, id);

                    map.Erase(map.Selected[position].Item1, position);
                    map.Selected[position] = (map.Marker.CreateMarkAtPosition(position), true);
                }
            }
            
            return removedObjects;
        }
    }
}