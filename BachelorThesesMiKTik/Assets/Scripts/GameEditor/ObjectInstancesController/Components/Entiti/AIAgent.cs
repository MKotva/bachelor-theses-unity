using Assets.Core.GameEditor.Components;
using Assets.Core.GameEditor.DTOS;
using Assets.Scenes.GameEditor.Core.AIActions;
using Assets.Scripts.GameEditor.AI.PathFind;
using Assets.Scripts.GameEditor.Entiti;
using Assets.Scripts.GameEditor.ObjectInstancesController;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameEditor.AI
{
    public class AIAgent : MonoBehaviour, IObjectController
    {
        public MapCanvas map;
        public AIObject AI;
        public IAIPathFinder pathFinder;

        private AIComponent aiSetting;
        private bool wasPlayed;
        private bool isPlaying;

        public void Initialize(AIComponent component)
        {
            aiSetting = component;
            AI = new AIObject(gameObject, component.Action.GetAction(gameObject));
        }

        public void Play()
        {
            if (!wasPlayed)
            {
                aiSetting.OnCreateAction.Execute(gameObject);
                wasPlayed = true;
            }
            isPlaying = true;
        }

        public void Pause()
        {
            isPlaying = false;
        }

        public void Enter() {}

        public void Exit()
        {
            isPlaying = false;
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
            if (TryGetComponent<ObjectController>(out var controller))
            {
                controller.Components.Add(typeof(ColliderController), this);
            }

            map = MapCanvas.Instance;
            pathFinder = new AStar();
        }

        private void FixedUpdate()
        {
            if (!isPlaying)
                return;

            if (aiSetting.OnUpdateAction != null)
                aiSetting.OnUpdateAction.Execute(gameObject);

            if (AI != null)
            {
                AI.PerformActions();
            }
        }
        #endregion
    }
}
