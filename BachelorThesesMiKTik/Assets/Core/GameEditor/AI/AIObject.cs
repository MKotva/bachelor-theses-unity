using Assets.Core.GameEditor.DTOS;
using Assets.Scenes.GameEditor.Core.AIActions;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.GameEditor.AI
{
    public class AIObject
    {
        public GameObject Performer { get; set; }
        public List<AIActionBase> Actions { get; internal set; }

        internal Queue<AgentActionDTO> actionsToPerform;
        internal Task performingTask;
        internal bool isPerforming;

        public AIObject(GameObject performer, List<AIActionBase> actions)
        {
            Performer = performer;
            Performer.GetComponent<BoxCollider2D>().enabled = true;

            Actions = actions;

            actionsToPerform = new Queue<AgentActionDTO>();
        }

        public virtual void AddActions(List<AgentActionDTO> actions)
        {
            actions.ForEach(action => actionsToPerform.Enqueue(action));
        }

        public virtual void PerformActions()
        {
            if (actionsToPerform.Count > 0 && !isPerforming)
            {
                var actualAction = actionsToPerform.Dequeue();
                performingTask = actualAction.Performer(actualAction);
                isPerforming = true;
            }
            else if (performingTask != null)
            {
                if (performingTask.IsCompleted)
                    isPerforming = false;
            }

        }

        public virtual bool IsActionFinished()
        {
            if (actionsToPerform.Count == 0)
                return true;
            return false;
        }
    }
}
