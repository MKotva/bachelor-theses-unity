using Assets.Core.GameEditor.DTOS;
using Assets.Scenes.GameEditor.Core.AIActions;
using Assets.Scripts.GameEditor.AI;
using Assets.Scripts.GameEditor.AI.PathFind;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.GameEditor.ObjectInstancesController.Components.Entiti
{
    public class ActionsAgent : MonoBehaviour
    {
        public GameObject Performer { get; set; }
        public List<ActionBase> Actions { get; internal set; }
        public EditorCanvas Map { get; internal set; }
        
        internal IAIPathFinder pathFinder;
        internal Queue<AgentActionDTO> actionsToPerform;
        internal Task performingTask;
        internal bool isPerforming;

        public virtual void EnqueueActions(List<AgentActionDTO> actions)
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

        public virtual bool HasActions()
        {
            if (actionsToPerform.Count == 0)
                return true;
            return false;
        }

        public virtual void ClearActions()
        {
            Actions.Clear();
        }

        public List<AgentActionDTO> FindPath(Vector3 endPosition)
        {
            return pathFinder.FindPath(gameObject.transform.position, endPosition, Actions);
        }

        public void MoveTo(Vector3 endPosition)
        {
            ClearActions();
            var path = pathFinder.FindPath(gameObject.transform.position, endPosition, Actions);
            if (path != null)
            {
                EnqueueActions(path);
            }
        }

        public List<GameObject> PintMoveTo(Vector3 endPosition)
        {
            var path = pathFinder.FindPath(gameObject.transform.position, endPosition, Actions);
            return AgentActionDTO.Print(path);
        }

        public List<GameObject> PrintPossibleActions()
        {
            List<GameObject> markers = new List<GameObject>();
            foreach (var action in Actions)
            {
                action.PrintReacheables(gameObject.transform.position).ForEach(x => markers.Add(x));
            }
            return markers;
        }

        protected virtual void Awake()
        {
            Performer = gameObject;
            Map = EditorCanvas.Instance;
            pathFinder = new AStar();

            if (Actions == null)
                Actions = new List<ActionBase>();
        }
    }
}
