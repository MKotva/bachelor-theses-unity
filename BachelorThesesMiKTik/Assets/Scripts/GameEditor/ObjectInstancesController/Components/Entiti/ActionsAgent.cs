using Assets.Core.GameEditor.DTOS;
using Assets.Scenes.GameEditor.Core.AIActions;
using Assets.Scripts.GameEditor.AI;
using Assets.Scripts.GameEditor.AI.PathFind;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.GameEditor.ObjectInstancesController.Components.Entiti
{
    public class ActionsAgent : MonoBehaviour
    {
        public GameObject Performer { get; set; }
        public List<ActionBase> ActionPerformers { get; internal set; }
        public EditorCanvas Map { get; internal set; }
        public Queue<AgentActionDTO> ActionsToPerform {get; internal set; }

        internal IAIPathFinder pathFinder;
        internal AgentActionDTO performingTask;
        internal bool isPerforming;

        public virtual void EnqueueActions(List<AgentActionDTO> actions)
        {
            actions.ForEach(action => ActionsToPerform.Enqueue(action));
        }

        public virtual void PerformActions()
        {
            if (ActionsToPerform.Count > 0 && performingTask == null)
            {
                performingTask = ActionsToPerform.Dequeue();
            }

            if (performingTask != null)
            {
                if(performingTask.Performer(performingTask, ActionsToPerform, Time.deltaTime))
                    performingTask = null;
            }
        }

        public virtual bool HasActions()
        {
            if (ActionsToPerform.Count == 0)
                return true;
            return false;
        }

        public virtual void ClearActions()
        {
            ActionsToPerform.Clear();
        }

        public void MoveTo(Vector3 endPosition)
        {
            ClearActions();

            var path = pathFinder.FindPath(Performer.transform.position, GetPosition(endPosition), ActionPerformers);
            if (path != null)
            {
                EnqueueActions(path);
            }
        }

        public List<GameObject> PrintMoveTo(Vector3 endPosition)
        {
            var path = pathFinder.FindPath(Performer.transform.position, GetPosition(endPosition), ActionPerformers);
            return AgentActionDTO.Print(path);
        }

        public void PerformRandomAction()
        {
            var actionPerformer = ActionPerformers[(int)Random.Range(0, ActionPerformers.Count - 1)];

            AgentActionDTO newAction = null;
            if (ActionsToPerform.Count != 0)
            {
                 newAction = actionPerformer.GetRandomAction(ActionsToPerform.Last().EndPosition);
            }
            else
            {
                newAction = actionPerformer.GetRandomAction(Performer.transform.position);
            }

            if (newAction != null && ActionsToPerform.Count < 1000) 
            {
                ActionsToPerform.Enqueue(newAction);
            }
        }

        public List<GameObject> PrintPossibleActions()
        {
            List<GameObject> markers = new List<GameObject>();
            foreach (var action in ActionPerformers)
            {
                var startPosition = GetPosition(gameObject.transform.position);
                action.PrintReacheables(startPosition).ForEach(x => markers.Add(x));
            }
            return markers;
        }

        private Vector2 GetPosition(Vector2 position)
        {
            return Map.GetCellCenterPosition(position);
        }

        protected virtual void Awake()
        {
            Performer = gameObject;
            Map = EditorCanvas.Instance;
            pathFinder = new AStar();
            ActionsToPerform = new Queue<AgentActionDTO>();

            if (ActionPerformers == null)
                ActionPerformers = new List<ActionBase>();
        }
    }
}
