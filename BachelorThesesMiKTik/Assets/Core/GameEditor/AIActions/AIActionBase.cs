using Assets.Core.GameEditor.DTOS;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scenes.GameEditor.Core.AIActions
{
    public abstract class AIActionBase
    {
        internal MapCanvasController context;
        internal float actionCost;

        public AIActionBase(MapCanvasController mapController, float cost = 1) 
        {
            context = mapController;
            actionCost = cost;
        }

        public abstract Task PerformActionAsync(AgentActionDTO action);
        public abstract Task<List<GameObject>> PrintActionAsync(AgentActionDTO action);
        public abstract List<AgentActionDTO> GetPossibleActions(Vector3 position); 
        
        public virtual bool IsPerforming()
        {
            throw new NotImplementedException();
        }

        public virtual List<Vector3> GetReacheablePositions(Vector3 position)
        {
            var positions = new List<Vector3>();
            foreach (var action in GetPossibleActions(position))
            {
                positions.Add(action.EndPosition);
            }
            return positions;
        }

        public virtual List<GameObject> PrintReacheables(Vector3 startPosition) 
        {
            List<GameObject> markers = new List<GameObject>();

            var color = UnityEngine.Random.ColorHSV();
            var reacheables = GetReacheablePositions(startPosition);
            
            foreach (var reacheable in reacheables)
                markers.Add(context.CreateMarkAtPosition(context.MarkerDotPrefab, reacheable, color));

            return markers;
        }

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
