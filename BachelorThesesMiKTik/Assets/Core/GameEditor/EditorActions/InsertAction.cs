using Assets.Core.GameEditor;
using Assets.Core.GameEditor.DTOS;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Scenes.GameEditor.Core.EditorActions
{
    public class InsertAction : EditorActionBase
    {
        bool _isMouseDown;
        private List<Vector3> _newObjectPostions; //All positions where object was created during one mouse press.
        private RemoveAction _removeAction;

        public InsertAction()
        {
            _newObjectPostions = new List<Vector3>();
            _removeAction = new RemoveAction(true); 
        }
        public InsertAction(bool dummy) { }
        public override void OnMouseDown(MouseButton key) 
        {
            lastActionRecord = null;
            lastActionRecordReverse = null;

            if (key == MouseButton.LeftMouse)
            {
                _isMouseDown = true;
                _newObjectPostions.Clear();
            }
        }

        public override void OnMouseUp() 
        {
            if (_isMouseDown)
            {
                var positionsString = GetPositionsString(_newObjectPostions);
                lastActionRecord = new JournalActionDTO($"I;{positionsString}", PerformAction);
                lastActionRecordReverse = new JournalActionDTO($"R;{positionsString}", _removeAction.PerformAction);
                _isMouseDown = false;
            }
        }

        public override void OnUpdate(Vector3 mousePosition) 
        {
            if(_isMouseDown) 
            {
                var position = map.GetCellCenterPosition(mousePosition);
                if (map.Selected.ContainsKey(position))
                {
                    InsertSelection();
                    lastActionRecord = new JournalActionDTO($"IS;{mousePosition.x}:{mousePosition.y}", PerformAction);
                    lastActionRecordReverse = new JournalActionDTO($"RS;{mousePosition.x}:{mousePosition.y}", _removeAction.PerformAction);
                    _isMouseDown = false;
                }
                else
                {
                    Insert(position);

                    if(!_newObjectPostions.Contains(position))
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

            if (descriptions[0] == "IS")
            {
                var position = MathHelper.GetVector3FromString(descriptions[1]);
                position = map.GetCellCenterPosition(position);

                if (map.Selected.ContainsKey(position))
                {
                    InsertSelection();
                }
            }
            else if (descriptions[0] == "I" && descriptions.Count() > 1)
            {
                for (int i = 1;  i < descriptions.Count(); i++) 
                {
                    if (descriptions[i] == "")
                        continue;

                    var position = MathHelper.GetVector3FromString(descriptions[i]);
                    Insert(position);
                }
            }
            else if (descriptions[0] == "IR" && descriptions.Count() > 1)
            {
                for (int i = 1; i < descriptions.Count(); i++)
                {
                    if (descriptions[i] == "")
                        continue;

                    var position = MathHelper.GetVector3FromString(descriptions[i]);
                    InsertReverse(position);
                }
            }
        }

        #region PRIVATE
        private void Insert(Vector3 position)
        {
            position = map.GetCellCenterPosition(position);
            GameObject objectAtPos = map.GetObjectAtPosition(position);
            if (objectAtPos == null)
            {
                map.Paint(map.ActualPrefab, position);
            }
        }

        private void InsertSelection()
        {
            var keys = map.Selected.Keys.ToArray();
            for (int i = 0; i < map.Selected.Count; i++)
            {
                var position = keys[i];
                if (map.Selected[position].Item2) //TODO: Check selection prefab.
                {
                    map.Erase(map.Selected[position].Item1);
                    
                    var newObject = map.Paint(map.ActualPrefab, position);
                    map.Marker.MarkObject(newObject);
                    map.Selected[position] = (newObject, false);
                }
            }

            _isMouseDown = false; //TODO: Check if this is good idea...
        }

        private void InsertReverse(Vector3 position)
        {
            position = map.GetCellCenterPosition(position);
            if (!map.Selected.ContainsKey(position))
                return;

            if (map.Selected[position].Item2)
            {
                map.Erase(map.Selected[position].Item1);

                var newObject = map.Paint(map.ActualPrefab, position);
                map.Marker.MarkObject(newObject);
                map.Selected[position] = (newObject, false);
            }
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
        #endregion
    }
}
