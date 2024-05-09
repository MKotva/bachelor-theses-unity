using Assets.Core.GameEditor.DTOS;
using Assets.Scenes.GameEditor.Core.AIActions;
using Assets.Scripts.GameEditor.AI;
using Assets.Scripts.GameEditor.AI.PathFind;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.GameEditor.ObjectInstancesController.Components.Entiti
{
    public class ActionsAgent : MonoBehaviour
    {
        public GameObject Performer { get; set; }
        public List<ActionBase> ActionPerformers { get; internal set; }
        public EditorCanvas Map { get; internal set; }
        public Queue<AgentActionDTO> ActionsToPerform { get; internal set; }
        public AgentActionDTO PerformingTask { get; internal set; }
        public Vector2 Velocity { get; internal set; }

        internal Vector2 originPosition;

        internal IAIPathFinder pathFinder;
        internal bool isPerforming;

        private int performCounter;
        private bool isSimulating;
        private Vector2 initialPosition;

        public virtual void EnqueueActions(List<AgentActionDTO> actions)
        {
            actions.ForEach(action => ActionsToPerform.Enqueue(action));
        }

        public virtual void PerformActions()
        {
            if (ActionsToPerform.Count > 0 && PerformingTask == null && isPerforming)
            {
                PerformingTask = ActionsToPerform.Dequeue();
                performCounter = 0;
            }

            if (PerformingTask != null)
            {
                if (PerformingTask.Action.PerformAgentAction(PerformingTask, ActionsToPerform, Time.deltaTime))
                    PerformingTask = null;
                else
                {
                    performCounter++;
                    CheckForStuck();
                }
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
            if (PerformingTask != null)
            {
                PerformingTask.Action.ClearAction();
                PerformingTask = null;
            }

            if (isSimulating)
            {
                EndSimulation();
            }
        }

        public void MoveTo(Vector3 endPosition)
        {
            if (isSimulating)
                return;

            ClearActions();

            var path = pathFinder.FindPath(Performer.transform.position, GetPosition(endPosition), ActionPerformers);
            if (path != null)
            {
                EnqueueActions(path);
            }
        }

        public void SimulateMoveTo(Vector3 endPosition)
        {
            if (isSimulating)
                return;

            ClearActions();

            var path = pathFinder.FindPath(Performer.transform.position, GetPosition(endPosition), ActionPerformers);
            if (path != null)
            {
                EnqueueActions(path);
            }

            initialPosition = transform.position;
            isPerforming = true;
            isSimulating = true;
        }

        public List<GameObject> PrintMoveTo(Vector3 endPosition)
        {
            if (isSimulating)
                return new List<GameObject>();

            var path = pathFinder.FindPath(Performer.transform.position, GetPosition(endPosition), ActionPerformers);
            return AgentActionDTO.Print(path);
        }

        public void PerformRandomAction()
        {
            var actionPerformer = ActionPerformers[(int) Random.Range(0, ActionPerformers.Count - 1)];

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
            Velocity = Vector2.zero;

            if (ActionPerformers == null)
                ActionPerformers = new List<ActionBase>();
        }

        protected virtual void FixedUpdate()
        {
            if (isSimulating)
            {
                if (ActionsToPerform.Count == 0 && PerformingTask == null)
                {
                    EndSimulation();
                }
                else
                {
                    PerformActions();
                }
            }
            else
            {
                var actualPosition = (Vector2) Performer.transform.position;
                Velocity = ( actualPosition - originPosition ) / Time.fixedDeltaTime;
                originPosition = actualPosition;

                foreach (var action in ActionPerformers)
                {
                    action.ClampSpeed();
                }
            }
        }

        private void EndSimulation()
        {
            isSimulating = false;
            isPerforming = false;
            transform.position = initialPosition;
        }

        private void CheckForStuck()
        {
            if (performCounter > 10000000)
            {
                PerformingTask.Action.ClearAction();
                transform.position = Map.GetCellCenterPosition(PerformingTask.EndPosition);
                PerformingTask = null;
                performCounter = 0;
            }
        }
    }
}
