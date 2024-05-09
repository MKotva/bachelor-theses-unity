using Assets.Scenes.GameEditor.Core.AIActions;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Core.GameEditor.DTOS
{
    public class AgentActionDTO
    {
        public delegate bool ActionPerformer(AgentActionDTO action, Queue<AgentActionDTO> actions, float deltaTime);
        public delegate Task<List<GameObject>> ActionPrintingPerformer(AgentActionDTO action);

        public Vector3 StartPosition {get; set;}
        public Vector3 EndPosition { get; set;}
        public string ActionParameters { get; set;}
        public float Cost { get; set; }

        public ActionBase Action { get; set;}

        public AgentActionDTO(Vector3 startPosition, Vector3 reachablePositions, string actionParameters, float cost, ActionBase action)
        {
            StartPosition = startPosition;
            EndPosition = reachablePositions;
            ActionParameters = actionParameters;
            Cost = cost;
            Action = action;
        }

        static public List<GameObject> Print(AgentActionDTO action) 
        {
            if (action == null)
                return new List<GameObject>();

            return action.Action.PrintAgentAction(action);
        }

        static public List<GameObject> Print(List<AgentActionDTO> path)
        {
            if (path == null)
                return new List<GameObject>();

            List<GameObject> markers = new List<GameObject>();
            foreach (var action in path)
            {
                Print(action).ForEach(x => markers.Add(x));   
            }
            return markers;
        }
    }
}
