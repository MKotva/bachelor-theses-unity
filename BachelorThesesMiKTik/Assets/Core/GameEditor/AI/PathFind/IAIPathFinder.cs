using Assets.Core.GameEditor.DTOS;
using Assets.Scenes.GameEditor.Core.AIActions;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameEditor.AI.PathFind
{
    public interface IAIPathFinder
    {
        public List<AgentActionDTO> FindPath(Vector3 startPosition, Vector3 endPosition, List<ActionBase> actions);
    }
}
