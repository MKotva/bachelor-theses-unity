using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
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

        public SelectAction(MapCanvasController context) : base(context){}

        public override void OnMouseDown(MouseButton button)
        {
            if (button == MouseButton.LeftMouse)
            {
                _isMouseDown = true;
                if (context.Selected.Count != 0)
                {
                    context.UnSelectAll();
                }
            }
        }

        public override void OnMouseUp()
        {
            _isMouseDown = false;
        }

        public override void OnKeyDown(Key key)
        {
            if (key == Key.LeftShift || key == Key.RightShift)
            {
                _squareStart = context.GetWorldMousePosition();
                _isKeyDown = true;
            }
        }

        public override void OnKeyUp()
        {
            _isKeyDown = false;
        }

        public override void OnUpdate(Vector3 mousePosition)
        {
            if (_isMouseDown)
            {
                if (_isKeyDown)
                {
                    SquareSelection(mousePosition);
                }
                else
                {
                    SingleSelection(mousePosition);
                }
            }
        }

        private void SingleSelection(Vector3 mousePosition)
        {
            var cellCenter = context.GetCellCenterPosition(mousePosition);
            var objectAtPositon = context.GetObjectAtPosition(cellCenter);

            if(objectAtPositon != null) 
            {
                if (context.Selected.ContainsKey(cellCenter))
                    return;

                context.MarkObject(objectAtPositon);
                context.Selected.Add(cellCenter, (objectAtPositon, false));
            }
        }

        private void SquareSelection(Vector3 position)
        {
            var xCellSize = context.GridLayout.cellSize.x;
            var yCellSize = context.GridLayout.cellSize.y;

            var xMove = (_squareStart.x - position.x) / xCellSize;
            var yMove = (_squareStart.y - position.y) / yCellSize;

            var xSign = Mathf.Sign(xMove);
            var ySign = Mathf.Sign(yMove);

            for (int i = 0; i < Math.Abs(xMove) + 1; i++)
            {
                for (int j = 0; j < Math.Abs(yMove) + 1; j++)
                {
                    var calculatedPosition = new Vector3(position.x + (xSign * i * xCellSize), position.y + ( ySign * j * yCellSize ));
                    var cellCenter = context.GetCellCenterPosition(calculatedPosition);
                    GameObject objectAtPos = context.GetObjectAtPosition(cellCenter);
                    
                    if (context.Selected.ContainsKey(cellCenter))
                        continue;

                    if (objectAtPos != null)
                    {
                        context.MarkObject(objectAtPos);
                        context.Selected.Add(cellCenter, (objectAtPos, false));
                    }
                    else
                    {
                        var newMarker = context.CreateMarkAtPosition(cellCenter);
                        context.Selected.Add(cellCenter, (newMarker, true));
                    }
                }
            }
        }
    }
}
