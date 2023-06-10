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
        private bool _isMouseDown;
        private bool _moveSelection;
        private Vector3 _startPosition;
        private Vector3 _lastMousePosition;
        private Vector3 _cameraOriginPosition;


        public MoveAction(MapCanvasController context) : base(context) { }

        public override void OnMouseDown(MouseButton key)
        {
            if (key == MouseButton.LeftMouse)
            {
                _isMouseDown = true;
                _startPosition = context.GetWorldMousePosition();
                var worldCellPosition = context.GetCellCenterPosition(_startPosition);
                if (context.Selected.ContainsKey(worldCellPosition))
                {
                    _moveSelection = true;
                }
                else
                {
                    _cameraOriginPosition = context.CameraObj.transform.position;
                }
            }
        }

        public override void OnMouseUp()
        {
            if (_moveSelection)
            {
                SaveToMove();
                _lastActionRecord = new JournalActionDTO($"MS;{_startPosition.x}:{_startPosition.y};{_lastMousePosition.x}:{_lastMousePosition.y}", PerformAction);
                _lastActionRecordReverse = new JournalActionDTO($"MS;{_lastMousePosition.x}:{_lastMousePosition.y};{_startPosition.x}:{_startPosition.y}", PerformAction);
                _moveSelection = false;
            }
            else
            {
                _lastActionRecord = new JournalActionDTO($"MC;{_startPosition.x}:{_startPosition.y};{_lastMousePosition.x}:{_lastMousePosition.y}", PerformAction);
                _lastActionRecordReverse = new JournalActionDTO($"MC;{_lastMousePosition.x}:{_lastMousePosition.y};{_startPosition.x}:{_startPosition.y}", PerformAction);
            }
            _isMouseDown = false;
        }

        public override void OnUpdate(Vector3 mousePosition)
        {
            if (_isMouseDown)
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
            if (_moveSelection)
                MoveSelected(_startPosition, position);
            else
                MoveMapView(_startPosition, position);

            _lastMousePosition = position;
        }

        public void MoveSelected(Vector3 fromPos, Vector3 toPos)
        {
            var xMove = fromPos.x - toPos.x;
            var yMove = fromPos.y - toPos.y;

            var newSelection = new Dictionary<Vector3, (GameObject, bool)>();
            var keys = context.Selected.Keys.ToArray();
            for (int i = 0; i < context.Selected.Count; i++)
            {
                var originPosition = keys[i];
                var originObjectInfo = context.Selected[originPosition];

                var newPosition = new Vector3(originPosition.x - xMove, originPosition.y - yMove);
                var newCellCenterPosition = context.GetCellCenterPosition(newPosition);

                originObjectInfo.Item1.transform.position = newCellCenterPosition;
            }
        }

        private void SaveToMove()
        {
            if (context.Selected.Count > 0)
            {
                var movedSelection = new Dictionary<Vector3, (GameObject, bool)>();
                var keys = context.Selected.Keys.ToArray();
                for (int i = 0; i < context.Selected.Count; i++)
                {
                    var originPosition = keys[i];
                    var objectInfo = context.Selected[originPosition];
                    var newPosition = objectInfo.Item1.transform.position;

                    if (!objectInfo.Item2)
                        context.ReplaceData(originPosition, newPosition, objectInfo.Item1);
                    movedSelection.Add(newPosition, objectInfo);
                }
                context.Selected = movedSelection;
            }
        }

        private void MoveMapView(Vector3 fromPosition, Vector3 position)
        {
            var xMove = ( fromPosition.x - position.x ) * 0.75f;
            var yMove = ( fromPosition.y - position.y ) * 0.75f;

            context.CameraObj.transform.position = new Vector3(_cameraOriginPosition.x + xMove, _cameraOriginPosition.y + yMove, _cameraOriginPosition.z);
        }
    }
}
