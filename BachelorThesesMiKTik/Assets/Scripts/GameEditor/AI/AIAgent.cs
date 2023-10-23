using Assets.Core.GameEditor.DTOS;
using Assets.Scripts.GameEditor.AI.PathFind;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.GameEditor.AI
{
    public abstract class AIAgent : MonoBehaviour
    {
        public MapCanvasController context;
        public AIObject AI;
        public IAIPathFinder pathFinder;   

        public virtual void OnAction()
        {
            AI.PerformActions();
        }

        public virtual void EnqueAction(List<AgentActionDTO> agentActions )
        {
            AI.AddActions(agentActions);
        }

        public abstract void Simulate();
        public abstract List<GameObject> PrintSimulation();
        public abstract List<GameObject> PrintPossibleActions();
    }
}
