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

        public RemoveAction(MapCanvasController context) : base(context) 
        {
            _newObjectPostions = new List<Vector3>();
            _insertAction = new InsertAction(context, true); 
        }
        public RemoveAction(MapCanvasController context, bool dummy) : base(context) 
        {
            _newObjectPostions = new List<Vector3>();
        }

        public override void OnMouseDown(MouseButton key)
        {
            if (key == MouseButton.LeftMouse)
            {
                _lastActionRecord = null;
                _lastActionRecordReverse = null;

                _isMouseDown = true;
                _newObjectPostions.Clear();
            }
        }

        public override void OnMouseUp()
        {
            if (_isMouseDown)
            {
                var positionsString = GetPositionsString(_newObjectPostions);
                _lastActionRecord = new JournalActionDTO($"R;{positionsString}", PerformAction);
                _lastActionRecordReverse = new JournalActionDTO($"I;{positionsString}", _insertAction.PerformAction);
                _isMouseDown = false;
            }
        }

        public override void OnUpdate(Vector3 mousePosition)
        {
            if (_isMouseDown)
            {
                var position = context.GetCellCenterPosition(mousePosition);
                if (context.Selected.ContainsKey(position))
                {
                    RemoveSelection();
                    var positionsString = GetPositionsString(_newObjectPostions);
                    _lastActionRecord = new JournalActionDTO($"RS;{position.x}:{position.y}", PerformAction);
                    _lastActionRecordReverse = new JournalActionDTO($"IR;{positionsString}", _insertAction.PerformAction);
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
                position = context.GetCellCenterPosition(position);

                if (context.Selected.ContainsKey(position))
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
            position = context.GetCellCenterPosition(position);
            GameObject objectAtPos = context.GetObjectAtPosition(position);
            if (objectAtPos != null)
            {
                context.Erase(objectAtPos, position);
            }
        }

        private void RemoveSelection()
        {
            var keys = context.Selected.Keys.ToArray();
            for (int i = 0; i < context.Selected.Count(); i++)
            {
                var position = keys[i];
                if (!context.Selected[position].Item2)
                {
                    context.Erase(context.Selected[position].Item1, position);
                    context.Selected[position] = (context.CreateMarkAtPosition(position), true);

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