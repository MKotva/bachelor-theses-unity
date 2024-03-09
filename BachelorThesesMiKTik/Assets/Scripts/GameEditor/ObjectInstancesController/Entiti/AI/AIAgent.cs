using Assets.Core.GameEditor.DTOS;
using Assets.Core.GameEditor.DTOS.Components;
using Assets.Core.SimpleCompiler;
using Assets.Scenes.GameEditor.Core.AIActions;
using Assets.Scripts.GameEditor.AI.PathFind;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameEditor.AI
{
    public class AIAgent : MonoBehaviour
    {
        public MapCanvas map;
        public AIObject AI;
        public IAIPathFinder pathFinder;

        private SimpleCode createAction;
        private SimpleCode updateAction;

        public void Initialize(AIComponentDTO component)
        {
            AI = new AIObject(gameObject, component.Action.GetAction(gameObject));
            createAction = component.OnCreateAction;
            updateAction = component.OnUpdateAction;
        }

        public virtual void EnqueAction(List<AgentActionDTO> agentActions)
        {
            AI.AddActions(agentActions);
        }

        public virtual void ClearActions()
        {
            AI.Actions.Clear();
        }

        public void AddActionType(AIActionBase actionBase)
        {
            AI.Actions.Add(actionBase);
        }

        public List<AgentActionDTO> FindPath(Vector3 endPosition)
        {
            return pathFinder.FindPath(gameObject.transform.position, endPosition, AI.Actions);
        }

        public void MoveTo(Vector3 endPosition)
        {
            ClearActions();
            var path = pathFinder.FindPath(gameObject.transform.position, endPosition, AI.Actions);
            if (path != null)
            {
                EnqueAction(path);
            }
        }

        public List<GameObject> PintMoveTo(Vector3 endPosition)
        {
            var path = pathFinder.FindPath(gameObject.transform.position, endPosition, AI.Actions);
            return AgentActionDTO.Print(path);
        }

        public List<GameObject> PrintPossibleActions()
        {
            List<GameObject> markers = new List<GameObject>();
            foreach (var action in AI.Actions)
            {
                action.PrintReacheables(gameObject.transform.position).ForEach(x => markers.Add(x));
            }
            return markers;
        }

        #region PRIVATE
        private void Awake()
        {
            map = MapCanvas.Instance;
            pathFinder = new AStar();

            if (createAction != null)
                createAction.Execute(gameObject);
        }

        private void FixedUpdate()
        {
            if (AI != null)
            {
                if(updateAction != null) 
                    updateAction.Execute(gameObject);
                AI.PerformActions();
            }
        }
        #endregion
    }
}
