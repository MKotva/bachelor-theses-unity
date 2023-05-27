using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Scenes.GameEditor.Core.EditorActions
{
    internal class MoveAction : EditorActionBase
    {
        bool _isMouseDown;
        bool _moveSelection;
        Vector3 _startPosition;
        Vector3 _cameraOriginPosition;
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
            _isMouseDown = false;
            _moveSelection = false;
            SaveToMove();
        }

        public override void OnUpdate(Vector3 mousePosition)
        {
            if (_isMouseDown)
            {
                Move(mousePosition);
            }
        }

        private void Move(Vector3 position)
        {
            if (_moveSelection)
                MoveSelected(position);
            else
                MoveMapView(position);
        }

        public void MoveSelected(Vector3 position)
        {
            var xMove = _startPosition.x - position.x;
            var yMove = _startPosition.y - position.y;

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

        private void MoveMapView(Vector3 position)
        {
            var screenToWorld = context.CameraObj.ScreenToWorldPoint(position);

            var xMove = (_startPosition.x - position.x) * 0.75f;
            var yMove = (_startPosition.y - position.y) * 0.75f;

            context.CameraObj.transform.position = new Vector3(_cameraOriginPosition.x + xMove, _cameraOriginPosition.y + yMove, _cameraOriginPosition.z);
        }

    }
}
