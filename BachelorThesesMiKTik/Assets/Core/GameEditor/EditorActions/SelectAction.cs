using Assets.Core.GameEditor;
using Assets.Core.GameEditor.DTOS;
using Assets.Core.GameEditor.DTOS.EditorActions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace Assets.Scenes.GameEditor.Core.EditorActions
{
    internal class SelectAction : EditorActionBase
    {
        private bool _isMouseDown;
        private bool _isKeyDown;
        private Vector3 _squareStart;
        private Vector3 _lastMousePosition;

        /// <summary>
        /// Handles mouse left button click. If there were selected objects, stores unselect and select 
        /// actions to journal.
        /// </summary>
        /// <param name="button"></param>
        public override void OnMouseDown(MouseButton button)
        {
            if (button == MouseButton.LeftMouse)
            {
                lastActionRecord = null;
                lastActionRecordReverse = null;

                _isMouseDown = true;
                if (map.Selected.Count != 0)
                { 
                    lastActionRecordReverse = new PositionOperation(SingleSelection, map.Selected.Keys.ToList());
                    map.UnselectAll();
                }
                else
                {
                    lastActionRecordReverse = new JournalActionDTO(UnselectAll);
                }
            }
        }

        /// <summary>
        /// Handles shift key press, by storing actual mouse position as start point for
        /// square selection.
        /// actions to journal.
        /// </summary>
        /// <param name="button"></param>
        public override void OnKeyDown(Key key)
        {
            if (key == Key.LeftShift || key == Key.RightShift)
            {
                _squareStart = map.GetWorldMousePosition();
                _isKeyDown = true;
            }
        }

        /// <summary>
        /// Handles mouse button release by storing all performed actions to Journal. 
        /// actions to journal.
        /// </summary>
        /// <param name="button"></param>
        public override void OnMouseUp()
        {
            if (_isMouseDown && !_isKeyDown)
            {
                lastActionRecord = new PositionOperation(SingleSelection, map.Selected.Keys.ToList());
                SaveRecord(lastActionRecord, lastActionRecordReverse);
            }
            else if (_isMouseDown && _isKeyDown)
            {
                lastActionRecord = new PositionOperation(SquareSelection, new List<Vector3> { _squareStart, _lastMousePosition });
                SaveRecord(lastActionRecord, lastActionRecordReverse);
            }

            _isMouseDown = false;
        }

        /// <summary>
        /// Handles key release by storing data about performed square selection action to Journal. 
        /// actions to journal.
        /// </summary>
        /// <param name="button"></param>
        public override void OnKeyUp()
        {
            _isKeyDown = false;
            _isMouseDown = false;
            lastActionRecord = new PositionOperation(SquareSelection, new List<Vector3> {_squareStart, _lastMousePosition });
            SaveRecord(lastActionRecord, lastActionRecordReverse);
        }


        /// <summary>
        /// Called on Unity Update call, based on given position and combination of
        /// pressed keys, performs single or square selection.
        /// </summary>
        /// <param name="mousePosition">Cursor position.</param>
        public override void OnUpdate(Vector3 mousePosition)
        {
            if (_isMouseDown)
            {
                if (_isKeyDown)
                {
                    SquareSelection(_squareStart, mousePosition);
                    _lastMousePosition = mousePosition;
                }
                else
                {
                    SingleSelection(mousePosition);
                }
            }
        }

        /// <summary>
        /// /// Based on given data from JournalAction, performs single selection.
        /// </summary>
        /// <param name="action"></param>
        public void SingleSelection(JournalActionDTO action)
        {
            if(action is PositionOperation)
            {
                map.UnselectAll();

                var positionOperation = (PositionOperation)action;
                foreach (var pos in positionOperation.Positions)
                {
                    SingleReselection(pos);
                }
            }
        }

        /// <summary>
        /// Based on given data from JournalAction, performs square selection.
        /// </summary>
        /// <param name="action"></param>
        public void SquareSelection(JournalActionDTO action)
        {
            if (action is PositionOperation)
            {
                map.UnselectAll();

                var positionOperation = (PositionOperation) action;
                if (positionOperation.Positions.Count == 2)
                {
                    SquareSelection(positionOperation.Positions[0], positionOperation.Positions[1]);
                }
            }
        }


        /// <summary>
        /// Unselects all selected objects.
        /// </summary>
        /// <param name="action"></param>
        public void UnselectAll(JournalActionDTO action)
        {
            map.UnselectAll();
        }

        #region PRIVATE

        /// <summary>
        /// Selects object on given position, if exists. If object is already selected
        /// nothing happens.
        /// </summary>
        /// <param name="mousePosition"></param>
        private void SingleSelection(Vector3 mousePosition)
        {
            var cellCenter = map.GetCellCenterPosition(mousePosition);
            if (map.Selected.ContainsKey(cellCenter))
                return;

            var objectAtPositon = map.GetObjectAtPosition(cellCenter);
            if (objectAtPositon != null)
            {
                map.Marker.MarkObject(objectAtPositon); //TODO : Initialize Marker, mb singleton?
                map.Selected.Add(cellCenter, (objectAtPositon, false));
            }
        }

        /// <summary>
        /// Selects object on given position, if exists or creates marker object.
        /// </summary>
        /// <param name="mousePosition"></param>
        private void SingleReselection(Vector3 mousePosition)
        {
            var cellCenter = map.GetCellCenterPosition(mousePosition);
            if (map.Selected.ContainsKey(cellCenter))
                return;

            var objectAtPositon = map.GetObjectAtPosition(cellCenter);
            if (objectAtPositon != null)
            {
                map.Marker.MarkObject(objectAtPositon);
                map.Selected.Add(cellCenter, (objectAtPositon, false));
            }
            else
            {
                var marker = map.Marker.CreateMarkAtPosition(cellCenter);
                map.Selected.Add(cellCenter, (marker, true));
            }
        }


        /// <summary>
        /// Selects objects in square between start and end vector. If there is no object present,
        /// method will create marker instead.
        /// </summary>
        /// <param name="fromPos"></param>
        /// <param name="toPos"></param>
        private void SquareSelection(Vector3 fromPos, Vector3 toPos)
        {
            map.UnselectAll();

            var xCellSize = map.GridLayout.cellSize.x;
            var yCellSize = map.GridLayout.cellSize.y;

            var xMove = ( fromPos.x - toPos.x ) / xCellSize;
            var yMove = ( fromPos.y - toPos.y ) / yCellSize;

            var xSign = Mathf.Sign(xMove);
            var ySign = Mathf.Sign(yMove);

            for (int i = 0; i < Math.Abs(xMove) + 1; i++)
            {
                for (int j = 0; j < Math.Abs(yMove) + 1; j++)
                {
                    var calculatedPosition = new Vector3(toPos.x + ( xSign * i * xCellSize ), toPos.y + ( ySign * j * yCellSize ));
                    var cellCenter = map.GetCellCenterPosition(calculatedPosition);
                    GameObject objectAtPos = map.GetObjectAtPosition(cellCenter);

                    if (map.Selected.ContainsKey(cellCenter))
                        continue;

                    if (objectAtPos != null)
                    {
                        map.Marker.MarkObject(objectAtPos);
                        map.Selected.Add(cellCenter, (objectAtPos, false));
                    }
                    else
                    {
                        var newMarker = map.Marker.CreateMarkAtPosition(cellCenter);
                        map.Selected.Add(cellCenter, (newMarker, true));
                    }
                }
            }
        }
        #endregion
    }
}
