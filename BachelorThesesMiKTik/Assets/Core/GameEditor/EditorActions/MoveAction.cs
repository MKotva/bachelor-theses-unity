using Assets.Core.GameEditor;
using Assets.Core.GameEditor.DTOS;
using Assets.Core.GameEditor.DTOS.EditorActions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Scenes.GameEditor.Core.EditorActions
{
    internal class MoveAction : EditorActionBase
    {
        private bool isMouseDown;
        private bool moveSelection;
        private Vector3 startPosition;
        private Vector3 lastMousePosition;
        private Vector3 cameraOriginPosition;

        private RemoveAction removeAction;
        private InsertAction insertAction;
        public MoveAction() : base()
        {
            removeAction = new RemoveAction();
            insertAction = new InsertAction();
        }

        /// <summary>
        /// Handles mouse left button click. If click is performed on selected position,
        /// this method will set actual action to move selection. Otherwise the actual action
        /// will be camera move.
        /// actions to journal.
        /// </summary>
        /// <param name="button"></param>
        public override void OnMouseDown(MouseButton key)
        {
            if (key == MouseButton.LeftMouse)
            {
                lastActionRecord = null;
                lastActionRecordReverse = null;

                isMouseDown = true;
                startPosition = map.GetWorldMousePosition();
                var worldCellPosition = map.GetCellCenterPosition(startPosition);
                if (map.Selected.ContainsKey(worldCellPosition))
                {
                    moveSelection = true;
                }
                else
                {
                    cameraOriginPosition = map.CameraObj.transform.position;
                }
            }
        }

        /// <summary>
        /// Handles mouse button release by saving performed action data to Journal.
        /// </summary>
        /// <param name="button"></param>
        public override void OnMouseUp()
        {
            if (!isMouseDown)
                return;

            if (moveSelection)
            {
                RecordOverride(SaveMove());
                lastActionRecord = new PositionOperation(MoveSelected, new List<Vector3> { startPosition, lastMousePosition });
                lastActionRecordReverse = new PositionOperation(MoveSelected, new List<Vector3> { lastMousePosition, startPosition });
                moveSelection = false;
            }
            else
            {
                lastActionRecord = new PositionOperation(MoveMapView, new List<Vector3> { startPosition, lastMousePosition });
                lastActionRecordReverse = new PositionOperation(MoveMapView, new List<Vector3> { lastMousePosition, startPosition });
            }

            SaveRecord(lastActionRecord, lastActionRecordReverse);
            isMouseDown = false;
        }

        /// <summary>
        /// Called on Unity Update call, based on given position,
        /// performs move action.
        /// </summary>
        /// <param name="mousePosition">Cursor position.</param>
        public override void OnUpdate(Vector3 mousePosition)
        {
            if (isMouseDown)
            {
                Move(mousePosition);
            }
        }

        /// <summary>
        /// Moves objects in selection based on given journal action.
        /// </summary>
        /// <param name="action"></param>
        public void MoveSelected(JournalActionDTO action)
        {
            if (action is PositionOperation)
            {
                var operationAction = (PositionOperation) action;
                if (operationAction.Positions.Count == 2)
                {
                    MoveSelected(operationAction.Positions[0], operationAction.Positions[1]);
                    SaveMove();
                }
            }
        }

        /// <summary>
        /// Moves objects in selection to new position. Direction of this translation
        /// is calculated as difference of given positions.
        /// </summary>
        /// <param name="fromPos">Start position</param>
        /// <param name="toPos">End position</param>
        public void MoveSelected(Vector3 fromPos, Vector3 toPos)
        {
            var xMove = fromPos.x - toPos.x;
            var yMove = fromPos.y - toPos.y;

            var keys = map.Selected.Keys.ToArray();
            for (int i = 0; i < map.Selected.Count; i++)
            {
                var originPosition = keys[i];
                var originObjectInfo = map.Selected[originPosition];

                var newPosition = new Vector3(originPosition.x - xMove, originPosition.y - yMove);
                var newCellCenterPosition = map.GetCellCenterPosition(newPosition);

                originObjectInfo.Item1.transform.position = newCellCenterPosition;
            }
        }

        /// <summary>
        /// Moves camera based on given journal action.
        /// </summary>
        /// <param name="action"></param>
        public void MoveMapView(JournalActionDTO action)
        {
            if (action is PositionOperation)
            {
                var operationAction = (PositionOperation) action;
                if (operationAction.Positions.Count == 2)
                {
                    MoveMapView(operationAction.Positions[0], operationAction.Positions[1]);
                }
            }
        }

        #region PRIVATE

        /// <summary>
        /// Moves map or selection in direction calculated from difference between
        /// click position and actual position of cursor.
        /// </summary>
        /// <param name="position"></param>
        private void Move(Vector3 position)
        {
            if (moveSelection)
                MoveSelected(startPosition, position);
            else
                MoveMapView(startPosition, position);

            lastMousePosition = position;
        }

        /// <summary>
        /// Saves positions of tranlated objects in selection to map Data. If there
        /// are collisions, old objects are removed and their data are stored to Journal.
        /// </summary>
        /// <returns></returns>
        private Dictionary<Vector3, int> SaveMove()
        {
            var removedObjects = new Dictionary<Vector3, int>();
            var newSelected = new Dictionary<Vector3, (GameObject, bool)>();
            if (map.Selected.Count > 0)
            {
                var keys = map.Selected.Keys.ToArray();
                for (int i = 0; i < map.Selected.Count; i++)
                {
                    var originPosition = keys[i];
                    var objectInfo = map.Selected[originPosition];
                    var newPosition = objectInfo.Item1.transform.position;

                    if (!objectInfo.Item2)
                    {
                        if (map.ContainsObjectAtPosition(newPosition, out int id))
                        {
                            removedObjects.Add(newPosition, id);
                        }
                        map.ReplaceData(originPosition, newPosition, objectInfo.Item1);
                    }

                    newSelected.Add(newPosition, objectInfo);
                }
            }
            map.Selected = newSelected;
            return removedObjects;
        }

        /// <summary>
        /// Creates journal action, if there were any removes objects during selection
        /// tranlation.
        /// </summary>
        /// <param name="removedObjects"></param>
        private void RecordOverride(Dictionary<Vector3, int> removedObjects)
        {
            if (removedObjects.Count > 0)
            {
                var removeRecord = new PositionOperation(removeAction.Remove, removedObjects.Keys.ToList());
                var insertRecord = new ItemOperation(insertAction.Insert, removedObjects);
                SaveRecord(removeRecord, insertRecord);
            }
        }

        /// <summary>
        /// Moves camera in direction of difference between two vectors. 
        /// </summary>
        /// <param name="fromPosition"></param>
        /// <param name="position"></param>
        private void MoveMapView(Vector3 fromPosition, Vector3 position)
        {
            var xMove = ( fromPosition.x - position.x ) * 0.75f;
            var yMove = ( fromPosition.y - position.y ) * 0.75f;

            map.CameraObj.transform.position = new Vector3(cameraOriginPosition.x + xMove, cameraOriginPosition.y + yMove, cameraOriginPosition.z);
        }

        #endregion
    }
}
