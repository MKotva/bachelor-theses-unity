using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Scenes.GameEditor.Core.EditorActions
{
    internal class MoveAction : EditorActionBase
    {
        bool _isMouseDown;
        Vector3 _startPosition;
        public MoveAction(GridController context) : base(context) { }

        public override void OnMouseDown(MouseButton key)
        {
            if (key == MouseButton.LeftMouse)
            {
                _isMouseDown = true;
                _startPosition = context.GetWorldMousePosition();
            }
        }

        public override void OnMouseUp()
        {
            _isMouseDown = false;
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
            var worldCellPosition = context.GetCellCenterPosition(position);
            if (context.Selected.ContainsKey(worldCellPosition))
            {
                MoveSelected(worldCellPosition);
            }

            MoveMapView();
        }

        public void MoveSelected(Vector3 position)
        {
            var xMove = _startPosition.x - position.x;
            var yMove = _startPosition.y - position.y;

            var xSign = Mathf.Sign(xMove);
            var ySign = Mathf.Sign(yMove);

            var xCellSize = context.GridLayout.cellSize.x;
            var yCellSize = context.GridLayout.cellSize.y;

            if(Math.Abs(xMove) > xCellSize || Math.Abs(yMove) > yCellSize)
            {
                foreach(var selected in context.Selected)
                {
                   var newPosition = new Vector3(selected.Key.x + xMove, selected.Key.y + yMove);
                   var cellCenterPosition = context.GetCellCenterPosition(newPosition);
                   selected.Value.Item1.transform.position = newPosition;
                }
            }
        }

        private void MoveMapView()
        {

        }
    }
}
