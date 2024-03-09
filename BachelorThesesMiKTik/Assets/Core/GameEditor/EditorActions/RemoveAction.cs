using Assets.Core.GameEditor;
using Assets.Core.GameEditor.DTOS;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Scenes.GameEditor.Core.EditorActions
{
    internal class RemoveAction : EditorActionBase
    {
        bool _isMouseDown;
        private List<Vector3> _newObjectPostions;
        private InsertAction _insertAction;

        public RemoveAction()
        {
            _newObjectPostions = new List<Vector3>();
            _insertAction = new InsertAction( true); 
        }
        public RemoveAction(bool dummy) 
        {
            _newObjectPostions = new List<Vector3>();
        }

        public override void OnMouseDown(MouseButton key)
        {
            if (key == MouseButton.LeftMouse)
            {
                lastActionRecord = null;
                lastActionRecordReverse = null;

                _isMouseDown = true;
                _newObjectPostions.Clear();
            }
        }

        public override void OnMouseUp()
        {
            if (_isMouseDown)
            {
                var positionsString = GetPositionsString(_newObjectPostions);
                lastActionRecord = new JournalActionDTO($"R;{positionsString}", PerformAction);
                lastActionRecordReverse = new JournalActionDTO($"I;{positionsString}", _insertAction.PerformAction);
                _isMouseDown = false;
            }
        }

        public override void OnUpdate(Vector3 mousePosition)
        {
            if (_isMouseDown)
            {
                var position = map.GetCellCenterPosition(mousePosition);
                if (map.Selected.ContainsKey(position))
                {
                    RemoveSelection();
                    var positionsString = GetPositionsString(_newObjectPostions);
                    lastActionRecord = new JournalActionDTO($"RS;{position.x}:{position.y}", PerformAction);
                    lastActionRecordReverse = new JournalActionDTO($"IR;{positionsString}", _insertAction.PerformAction);
                    _isMouseDown = false;
                }
                else
                {
                    Remove(position);

                    if (!_newObjectPostions.Contains(position))
                        _newObjectPostions.Add(position);
                }    
            }
        }

        public override void PerformAction(string action)
        {
            var descriptions = action.Split(';');
            if (descriptions.Length < 1)
            {
                return;
            }

            if (descriptions[0] == "RS")
            {
                var position = MathHelper.GetVector3FromString(descriptions[1]);
                position = map.GetCellCenterPosition(position);

                if (map.Selected.ContainsKey(position))
                {
                    RemoveSelection();
                }
            }
            else if (descriptions[0] == "R" && descriptions.Count() > 1)
            {
                for (int i = 1; i < descriptions.Count(); i++)
                {
                    if (descriptions[i] == "")
                        continue;

                    var position = MathHelper.GetVector3FromString(descriptions[i]);
                    Remove(position);
                }
            }
        }

        private void Remove(Vector3 position)
        {
            position = map.GetCellCenterPosition(position);
            GameObject objectAtPos = map.GetObjectAtPosition(position);
            if (objectAtPos != null)
            {
                map.Erase(objectAtPos, position);
            }
        }

        private void RemoveSelection()
        {
            var keys = map.Selected.Keys.ToArray();
            for (int i = 0; i < map.Selected.Count(); i++)
            {
                var position = keys[i];
                if (!map.Selected[position].Item2)
                {
                    map.Erase(map.Selected[position].Item1, position);
                    map.Selected[position] = (map.Marker.CreateMarkAtPosition(position), true);

                    if (!_newObjectPostions.Contains(position))
                        _newObjectPostions.Add(position);
                }
            }
            _isMouseDown = false;
        }

        private string GetPositionsString(List<Vector3> positions)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var pos in positions)
            {
                sb.Append($"{pos.x}:{pos.y};");
            }
            return sb.ToString();
        }
    }
}