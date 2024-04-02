using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Core.GameEditor.DTOS
{
    public class AgentActionDTO
    {
        public delegate Task ActionPerformer(AgentActionDTO action);
        public delegate Task<List<GameObject>> ActionPrintingPerformer(AgentActionDTO action);

        public Vector3 StartPosition {get; set;}
        public Vector3 EndPosition { get; set;}
        public bool IsDone { get; set; }
        public string ActionParameters { get; set;}
        public float Cost { get; set;}
        public ActionPerformer Performer { get; set;}
        public ActionPrintingPerformer PrintingPerformer { get; set;}

        public AgentActionDTO(Vector3 startPosition, Vector3 reachablePositions, string actionParameters, float cost, ActionPerformer actionPerformer, ActionPrintingPerformer printtingPerformer)
        {
            StartPosition = startPosition;
            EndPosition = reachablePositions;
            ActionParameters = actionParameters;
            Cost = cost;
            Performer = actionPerformer;
            PrintingPerformer = printtingPerformer;
        }

        static public List<GameObject> Print(AgentActionDTO action) 
        {
            if (action == null)
                return new List<GameObject>();

            return action.PrintingPerformer(action).Result;
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
