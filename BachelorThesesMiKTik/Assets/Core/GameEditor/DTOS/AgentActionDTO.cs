using System.Collections.Generic;
using UnityEngine;

namespace Assets.Core.GameEditor.DTOS
{
    public class AgentActionDTO
    {
        public delegate void ActionPerformer(AgentActionDTO action);
        public delegate List<GameObject> ActionPrintingPerformer(AgentActionDTO action);

        public Vector3 StartPosition;
        public Vector3 EndPosition;
        public string PositionActionParameter;
        public float Cost;
        public ActionPerformer Performer;
        public ActionPrintingPerformer PrintingPerformer;

        public AgentActionDTO(Vector3 startPosition, Vector3 reachablePositions, string positionActionParameters, float cost, ActionPerformer actionPerformer, ActionPrintingPerformer printtingPerformer)
        {
            StartPosition = startPosition;
            EndPosition = reachablePositions;
            PositionActionParameter = positionActionParameters;
            Cost = cost;
            Performer = actionPerformer;
            PrintingPerformer = printtingPerformer;
        }
    }
}
