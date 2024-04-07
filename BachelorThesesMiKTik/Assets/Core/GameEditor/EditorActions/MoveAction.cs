using Assets.Core.GameEditor;
using Assets.Core.GameEditor.DTOS;
using System;
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

        public override void OnMouseUp()
        {
            if (moveSelection)
            {
                SaveToMove();
                lastActionRecord = new JournalActionDTO($"MS;{startPosition.x}:{startPosition.y};{lastMousePosition.x}:{lastMousePosition.y}", PerformAction);
                lastActionRecordReverse = new JournalActionDTO($"MS;{lastMousePosition.x}:{lastMousePosition.y};{startPosition.x}:{startPosition.y}", PerformAction);
                moveSelection = false;
            }
            else
            {
                lastActionRecord = new JournalActionDTO($"MC;{startPosition.x}:{startPosition.y};{lastMousePosition.x}:{lastMousePosition.y}", PerformAction);
                lastActionRecordReverse = new JournalActionDTO($"MC;{lastMousePosition.x}:{lastMousePosition.y};{startPosition.x}:{startPosition.y}", PerformAction);
            }
            isMouseDown = false;
        }

        public override void OnUpdate(Vector3 mousePosition)
        {
            if (isMouseDown)
            {
                Move(mousePosition);
            }
        }

        public override void PerformAction(string action)
        {
            var descriptions = action.Split(';');
            var fromPos = MathHelper.GetVector3FromString(descriptions[1]);
            var toPos = MathHelper.GetVector3FromString(descriptions[2]);

            if (descriptions[0] == "MS")
            {
                MoveSelected(fromPos, toPos);
                SaveToMove();
            }
            else if (descriptions[0] == "MC")
            {
                MoveMapView(fromPos, toPos);
            }
        }

        private void Move(Vector3 position)
        {
            if (moveSelection)
                MoveSelected(startPosition, position);
            else
                MoveMapView(startPosition, position);

            lastMousePosition = position;
        }

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

        private void SaveToMove()
        {
            if (map.Selected.Count > 0)
            {
                var movedSelection = new Dictionary<Vector3, (GameObject, bool)>();
                var keys = map.Selected.Keys.ToArray();
                for (int i = 0; i < map.Selected.Count; i++)
                {
                    var originPosition = keys[i];
                    var objectInfo = map.Selected[originPosition];
                    var newPosition = objectInfo.Item1.transform.position;

                    if (!objectInfo.Item2)
                        map.ReplaceData(originPosition, newPosition, objectInfo.Item1);
                    movedSelection.Add(newPosition, objectInfo);
                }
                map.Selected = movedSelection;
            }
        }

        private void MoveMapView(Vector3 fromPosition, Vector3 position)
        {
            var xMove = ( fromPosition.x - position.x ) * 0.75f;
            var yMove = ( fromPosition.y - position.y ) * 0.75f;

            map.CameraObj.transform.position = new Vector3(cameraOriginPosition.x + xMove, cameraOriginPosition.y + yMove, cameraOriginPosition.z);
        }
    }
}
