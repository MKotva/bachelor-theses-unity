using Assets.Core.GameEditor.DTOS;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Core.GameEditor.Journal
{
    public class Journal
    {
        private int _capacity;

        private Stack<(JournalActionDTO, JournalActionDTO)> performedActions;
        private Stack<(JournalActionDTO, JournalActionDTO)> undoedActions;

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

            performedActions = new Stack<(JournalActionDTO, JournalActionDTO)>();
            undoedActions = new Stack<(JournalActionDTO, JournalActionDTO)>();
        }

        /// <summary>
        /// Creates record of performed action. Stores action and counter 
        /// action (performed, undoed).
        /// </summary>
        /// <param name="performedAction"></param>
        /// <param name="inverseAction"></param>
        public void Record(JournalActionDTO performedAction, JournalActionDTO inverseAction)
        {
            if(undoedActions.Count != 0)
            {
                undoedActions.Clear();
            }
            if(performedActions.Count == _capacity) 
            {
                CutHalfRecords();
            }
            performedActions.Push((performedAction, inverseAction));
        }

        /// <summary>
        /// If there are some performed actions, takes last of them and returns its 
        /// counter action. 
        /// </summary>
        /// <returns></returns>
        public JournalActionDTO GetUndoAction()
        {
            if (performedActions.Count == 0)
                return null;

            var actions = performedActions.Pop();
            undoedActions.Push(actions);
            return actions.Item2;
        }

        /// <summary>
        /// If there are some undoed actions, takes last of them and returns its 
        /// counter action. 
        /// </summary>
        /// <returns></returns>
        public JournalActionDTO GetRedoAction() 
        {
            if(undoedActions.Count == 0)
                return null;

            var actions = undoedActions.Pop();
            performedActions.Push(actions);
            return actions.Item1;
        }

        /// <summary>
        /// Resets journal to initial state
        /// </summary>
        public void Clear()
        {
            performedActions.Clear();
            undoedActions.Clear();
        }

        /// <summary>
        /// Cuts performed action in half and stores the newest actions.
        /// </summary>
        private void CutHalfRecords()
        {
            var halfIndex = performedActions.Count / 2;
            performedActions = new Stack<(JournalActionDTO, JournalActionDTO)>(performedActions.ToList().GetRange(0, halfIndex));
        }
    }
}
