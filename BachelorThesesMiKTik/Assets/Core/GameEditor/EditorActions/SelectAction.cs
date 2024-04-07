using Assets.Core.GameEditor;
using Assets.Core.GameEditor.DTOS;
using System;
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

        public override void OnMouseDown(MouseButton button)
        {
            if (button == MouseButton.LeftMouse)
            {
                lastActionRecord = null;
                lastActionRecordReverse = null;

                _isMouseDown = true;
                if (map.Selected.Count != 0)
                {
                    var positions = GetSelectedPositionsString();
                    lastActionRecordReverse = new JournalActionDTO($"SS;{positions}", PerformAction);
                    map.UnselectAll();
                }
                else
                {
                    lastActionRecordReverse = new JournalActionDTO($"SUA", PerformAction);
                }
            }
        }

        public override void OnMouseUp()
        {
            if (_isMouseDown && !_isKeyDown)
            {
                var positions = GetSelectedPositionsString();
                lastActionRecord = new JournalActionDTO($"SS;{positions}", PerformAction);
            }
            else if (_isMouseDown && _isKeyDown)
            {
                lastActionRecord = new JournalActionDTO($"SSQ;{_squareStart.x}:{_squareStart.y};{_lastMousePosition.x}:{_lastMousePosition.y}", PerformAction);
            }

            _isMouseDown = false;
        }

        public override void OnKeyDown(Key key)
        {
            if (key == Key.LeftShift || key == Key.RightShift)
            {
                _squareStart = map.GetWorldMousePosition();
                _isKeyDown = true;
            }
        }

        public override void OnKeyUp()
        {
            _isKeyDown = false;
            _isMouseDown = false;
            lastActionRecord = new JournalActionDTO($"SSQ;{_squareStart.x}:{_squareStart.y};{_lastMousePosition.x}:{_lastMousePosition.y}", PerformAction);
        }

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

        public override void PerformAction(string action)
        {
            var descriptions = action.Split(';');
            if (descriptions.Length < 1)
            {
                ErrorOutputManager.Instance.ShowMessage("Journal action parsing error!");
                return;
            }

            if (descriptions[0] == "SS")
            {
                if (descriptions.Length < 2)
                    return;

                map.UnselectAll();
                for (int i = 1; i < descriptions.Length; i++)
                {
                    if (descriptions[i] == "")
                        continue;

                    SingleReselection(MathHelper.GetVector3FromString(descriptions[i]));
                }
            }
            else if (descriptions[0] == "SSQ")
            {
                if (descriptions.Length != 3)
                    return;

                var fromPos = MathHelper.GetVector3FromString(descriptions[1]);
                var toPos = MathHelper.GetVector3FromString(descriptions[2]);
                SquareSelection(fromPos, toPos);
            }
            else if (descriptions[0] == "SUA")
            {
                map.UnselectAll();
            }
        }

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

        private string GetSelectedPositionsString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var selectedPos in map.Selected.Keys)
            {
                sb.Append($"{selectedPos.x}:{selectedPos.y};");
            }
            return sb.ToString();
        }
    }
}
