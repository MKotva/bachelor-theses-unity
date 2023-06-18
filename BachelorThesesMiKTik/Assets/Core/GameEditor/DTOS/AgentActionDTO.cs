using System.Collections.Generic;
using UnityEngine;

namespace Assets.Core.GameEditor.DTOS
{
    public class AgentActionDTO
    {
        public delegate void AgentAction(string parametes);

        public List<Vector3> ReachablePositions;
        public List<string> PositionActionParameters;
        public AgentAction ActionPerformer;
        public float Cost;

        public AgentActionDTO(List<Vector3> reachablePositions, List<string> positionActionParameters, AgentAction actionPerformer, float cost)
        {
            ReachablePositions = reachablePositions;
            PositionActionParameters = positionActionParameters;
            Cost = cost;
        }
    }
}
