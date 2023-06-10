using Assets.Core.GameEditor.DTOS;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Core.GameEditor.Journal
{
    public class Journal
    {
        private int _capacity;

        private Stack<(JournalActionDTO, JournalActionDTO)> _performedActions;
        private Stack<(JournalActionDTO, JournalActionDTO)> _undoedActions;

        public Journal(int capacity) 
        {
            if (capacity < 1)
            {
                _capacity = 10;
            }
            else
            {
                _capacity = capacity;
            }

            _performedActions = new Stack<(JournalActionDTO, JournalActionDTO)>();
            _undoedActions = new Stack<(JournalActionDTO, JournalActionDTO)>();
        }

        public void Record(JournalActionDTO performedAction, JournalActionDTO inverseAction)
        {
            if(_undoedActions.Count != 0)
            {
                _undoedActions.Clear();
            }
            if(_performedActions.Count == _capacity) 
            {
                CutHalfRecords();
            }
            _performedActions.Push((performedAction, inverseAction));
        }


        public JournalActionDTO GetUndoAction()
        {
            if (_performedActions.Count == 0)
                return null;

            var actions = _performedActions.Pop();
            _undoedActions.Push(actions);
            return actions.Item2;
        }

        public JournalActionDTO GetRedoAction() 
        {
            if(_undoedActions.Count == 0)
                return null;

            var actions = _undoedActions.Pop();
            _performedActions.Push(actions);
            return actions.Item1;
        }

        public void Clear()
        {
            _performedActions.Clear();
            _undoedActions.Clear();
        }

        private void CutHalfRecords()
        {
            var halfIndex = _performedActions.Count / 2;
            _performedActions = new Stack<(JournalActionDTO, JournalActionDTO)>(_performedActions.ToList().GetRange(0, halfIndex));
        }
    }
}
