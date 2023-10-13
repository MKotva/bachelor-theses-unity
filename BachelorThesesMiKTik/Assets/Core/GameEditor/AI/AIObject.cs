using Assets.Core.GameEditor.DTOS;
using Assets.Scenes.GameEditor.Core.AIActions;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameEditor.AI
{
    public class AIObject
    {
        public GameObject AIObjectl { get; set; }
        public List<AIActionBase> Actions { get; internal set; }


        internal Queue<AgentActionDTO> _actionsToPerform;
        internal bool _isPerforming;

        public AIObject(GameObject aIObjectl, List<AIActionBase> actions)
        {
            AIObjectl = aIObjectl;
            Actions = actions;
        }

        public virtual void AddActions(List<AgentActionDTO> actions)
        {
            actions.ForEach(action => _actionsToPerform.Enqueue(action));
            _isPerforming = true;
        }

        public virtual void PerformActions()
        {
            if (_isPerforming && _actionsToPerform.Count > 0)
            {
                if (_actionsToPerform.Count > 0)
                {
                    var actualAction = _actionsToPerform.Dequeue();
                    actualAction.Performer(actualAction);
                }
                else
                {
                    _isPerforming = false;
                }
            }
        }

        public virtual bool IsActionFinished()
        {
            if (_actionsToPerform.Count == 0)
                return true;
            return false;
        }
    }
}
