using Assets.Core.GameEditor.DTOS;
using Assets.Core.GameEditor.DTOS.EditorActions;
using Assets.Scripts.GameEditor.Managers;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Scenes.GameEditor.Core.EditorActions
{
    public class InsertAction : EditorActionBase
    {
        private Dictionary<Vector3, int> _newObjectPostions; //All positions where object was created during one mouse press.
        private RemoveAction _removeAction;

        private bool _isMouseDown;
        private bool _SelectionInsert;
        private Vector3 _mousePosition;

        public InsertAction()
        {
            _newObjectPostions = new Dictionary<Vector3, int>();
            _removeAction = new RemoveAction(true);
        }
        public InsertAction(bool dummy) { }

        /// <summary>
        /// Handles mouse left button click by re-initializing action data.
        /// actions to journal.
        /// </summary>
        /// <param name="button"></param>
        public override void OnMouseDown(MouseButton key)
        {
            if (key == MouseButton.LeftMouse)
            {
                _isMouseDown = true;
                _SelectionInsert = false;
                _newObjectPostions = new Dictionary<Vector3, int>();
            }
        }

        /// <summary>
        /// Handles mouse button release by saving performed action data to Journal.
        /// </summary>
        /// <param name="button"></param>
        public override void OnMouseUp()
        {
            if (_isMouseDown || _SelectionInsert)
            {
                if (_SelectionInsert)
                {
                    lastActionRecord = new ItemOperation(InsertSelection, new Dictionary<Vector3, int> { { _mousePosition, map.ActualPrefab.Id } });
                    lastActionRecordReverse = new PositionOperation(_removeAction.RemoveSelection, new List<Vector3> { _mousePosition });
                }
                else
                {
                    lastActionRecord = new ItemOperation(Insert, _newObjectPostions);
                    lastActionRecordReverse = new PositionOperation(_removeAction.Remove, _newObjectPostions.Keys.ToList());
                }
                SaveRecord(lastActionRecord, lastActionRecordReverse);
                _isMouseDown = false;
                _SelectionInsert = false;
            }
        }

        /// <summary>
        /// Called on Unity Update call, based on given position,
        /// performs insert or insert selection action.
        /// </summary>
        /// <param name="mousePosition">Cursor position.</param>
        public override void OnUpdate(Vector3 mousePosition)
        {
            if (_isMouseDown)
            {
                var position = map.GetCellCenterPosition(mousePosition);
                if (map.Selected.ContainsKey(position))
                {
                    InsertSelection(map.ActualPrefab);
                    _mousePosition = mousePosition;
                    _SelectionInsert = true;
                    _isMouseDown = false;
                }
                else
                {
                    if (Insert(position, map.ActualPrefab))
                        _newObjectPostions.Add(position, map.ActualPrefab.Id);
                }
            }
        }

        /// <summary>
        /// Inserts objects to map based on given journal action.
        /// </summary>
        /// <param name="action">Journal action</param>
        public void Insert(JournalActionDTO action)
        {
            if (action is ItemOperation)
            {
                var itemManager = ItemManager.Instance;
                if (itemManager == null)
                    return;

                var itemOperation = (ItemOperation) action;
                foreach (var position in itemOperation.Items.Keys)
                {
                    var id = itemOperation.Items[position];
                    if(itemManager.Items.ContainsKey(id))
                    {
                        Insert(position, itemManager.Items[id]);
                    }
                    else
                    {
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Inserts objects to selection on position from given journal action.
        /// </summary>
        /// <param name="action">Journal action</param>
        public void InsertSelection(JournalActionDTO action)
        {
            if (action is ItemOperation)
            {
                var itemOperation = (ItemOperation) action;
                if (itemOperation.Items.Count != 1)
                    return;

                var itemManager = ItemManager.Instance;
                if (itemManager == null)
                    return;

                var position = itemOperation.Items.Keys.First();
                var centeredPosition = map.GetCellCenterPosition(position);
                var id = itemOperation.Items[position];
                if (itemManager.Items.ContainsKey(id) && map.Selected.ContainsKey(centeredPosition))
                {
                    InsertSelection(itemManager.Items[id]);
                }
            }
        }

        /// <summary>
        /// Inserts objects and marks them as selected, based on given journal action.
        /// </summary>
        /// <param name="action">Journal action</param>
        public void InsertAsSelected(JournalActionDTO action)
        {
            if (action is ItemOperation)
            {
                var itemManager = ItemManager.Instance;
                if (itemManager == null)
                    return;

                var itemOperation = (ItemOperation) action;
                foreach (var position in itemOperation.Items.Keys)
                {
                    var id = itemOperation.Items[position];
                    if (itemManager.Items.ContainsKey(id))
                    {
                        InsertReverse(itemManager.Items[id], position);
                    }
                    else
                    {
                        return;
                    }
                }
            }
        }

        #region PRIVATE
        /// <summary>
        /// Creates new object of given prototype on given position.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="prototype"></param>
        /// <returns></returns>
        private bool Insert(Vector3 position, ItemData prototype)
        {
            position = map.GetCellCenterPosition(position);
            if (!map.ContainsObjectAtPosition(position))
            {
                map.Paint(prototype, position);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Inserts new objects of given prototype to empty positions in selection
        /// </summary>
        /// <param name="prototype"></param>
        private void InsertSelection(ItemData prototype)
        {
            var keys = map.Selected.Keys.ToArray();
            for (int i = 0; i < map.Selected.Count; i++)
            {
                var position = keys[i];
                if (map.Selected[position].Item2) //TODO: Check selection prefab.
                {
                    map.Erase(map.Selected[position].Item1);

                    var newObject = map.Paint(prototype, position);
                    map.Marker.MarkObject(newObject);
                    map.Selected[position] = (newObject, false);
                }
            }

            _isMouseDown = false;
        }

        /// <summary>
        /// Creates new object of given prototype on given position. Then this
        /// object marks as selected.
        /// </summary>
        /// <param name="prototype"></param>
        /// <param name="position"></param>
        private void InsertReverse(ItemData prototype, Vector3 position)
        {
            position = map.GetCellCenterPosition(position);
            if (!map.Selected.ContainsKey(position))
                return;

            if (map.Selected[position].Item2)
            {
                map.Erase(map.Selected[position].Item1);

                var newObject = map.Paint(prototype, position);
                map.Marker.MarkObject(newObject);
                map.Selected[position] = (newObject, false);
            }
        }
        #endregion
    }
}
