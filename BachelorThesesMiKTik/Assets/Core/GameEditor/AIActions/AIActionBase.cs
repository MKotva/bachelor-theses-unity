using Assets.Core.GameEditor.DTOS;
using Mono.Cecil;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Assets.Scenes.GameEditor.Core.AIActions
{
    public class AIActionBase
    {
        internal MapCanvasController context;
        internal float actionCost;

        public AIActionBase(MapCanvasController mapController, float cost = 1) 
        {
            context = mapController;
            actionCost = cost;
        }

        public virtual AgentActionDTO GetReacheablePosition(Vector3 position) {  return null; }
        public virtual void PerformAction(string parameters) { }

        internal bool IsWalkable(Vector3 position)
        {
            if (context.ContainsBlockingObjectAtPosition(position))
                return false;

            var _cellSize = context.GridLayout.cellSize;
            var lowerNeighbourPosition = context.GetCellCenterPosition(new Vector3(position.x, position.y - _cellSize.y));
            if (!context.ContainsObjectAtPosition(lowerNeighbourPosition))
                return false;

            var item = context.GetObjectAtPosition(lowerNeighbourPosition);
            if (item.layer != 7)
                return false;

            return true;
        }
    }
}
