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

        public InsertAction(MapCanvasController context) : base(context) 
        {
            _newObjectPostions = new List<Vector3>();
            _removeAction = new RemoveAction(context, true); 
        }
        public InsertAction(MapCanvasController context, bool dummy) : base(context) {}
        public override void OnMouseDown(MouseButton key) 
        {
            if (key == MouseButton.LeftMouse)
            {
                _isMouseDown = true;
                _newObjectPostions.Clear();
            }
        }

        public override void OnMouseUp() 
        {
            _isMouseDown = false;
            var positionsString = GetPositionsString(_newObjectPostions);
            _lastActionRecord = new JournalActionDTO($"I;{positionsString}", PerformAction);
            _lastActionRecordReverse = new JournalActionDTO($"R;{positionsString}", _removeAction.PerformAction);
        }

        public override void OnUpdate(Vector3 mousePosition) 
        {
            if(_isMouseDown) 
            {
                var position = context.GetCellCenterPosition(mousePosition);
                if (context.Selected.ContainsKey(position))
                {
                    InsertSelection();
                    _lastActionRecordReverse = new JournalActionDTO($"IS;{mousePosition.x}:{mousePosition.y}", PerformAction);
                    _lastActionRecord = new JournalActionDTO($"RS;{mousePosition.x}:{mousePosition.y}", _removeAction.PerformAction);
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
                position = context.GetCellCenterPosition(position);

                if (context.Selected.ContainsKey(position))
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
        }

        private void Insert(Vector3 position)
        {
            position = context.GetCellCenterPosition(position);
            GameObject objectAtPos = context.GetObjectAtPosition(position);
            if (objectAtPos == null)
            {
                var newObject = context.Paint(context.ActualPrefab.Prefab, context.Parent, context.GridLayout, position);
                context.InsertToData(position, newObject);
            }
        }

        private void InsertSelection()
        {
            var keys = context.Selected.Keys.ToArray();
            for (int i = 0; i < context.Selected.Count; i++)
            {
                var position = keys[i];
                if (context.Selected[position].Item2) //TODO: Check selection prefab.
                {
                    context.Erase(context.Selected[position].Item1);
                    
                    var newObject = context.Paint(context.ActualPrefab.Prefab, context.Parent, context.GridLayout, position);
                    context.MarkObject(newObject);
                    context.Selected[position] = (newObject, false);
                    context.InsertToData(position, newObject);
                }
            }

            _isMouseDown = false; //TODO: Check if this is good idea...
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
