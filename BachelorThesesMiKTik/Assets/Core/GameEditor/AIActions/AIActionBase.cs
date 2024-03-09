using Assets.Core.GameEditor.DTOS;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Scenes.GameEditor.Core.AIActions
{
    public abstract class AIActionBase
    {
        internal MapCanvas map;
        internal float actionCost;

        public AIActionBase(float cost = 1) 
        {
            map = MapCanvas.Instance;
            actionCost = cost;
        }

        public abstract void PerformAction(string action);
        public abstract Task PerformActionAsync(AgentActionDTO action);
        public abstract Task<List<GameObject>> PrintActionAsync(AgentActionDTO action);
        public abstract List<AgentActionDTO> GetPossibleActions(Vector3 position);
        public abstract bool IsPerforming();

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
                markers.Add(map.Marker.CreateMarkAtPosition(map.Marker.MarkerDotPrefab, reacheable, color));

            return markers;
        }

        internal bool IsWalkable(Vector3 position)
        {
            if (map.ContainsBlockingObjectAtPosition(position))
                return false;

            var _cellSize = map.GridLayout.cellSize;
            var lowerNeighbourPosition = map.GetCellCenterPosition(new Vector3(position.x, position.y - _cellSize.y));
            if (!map.ContainsObjectAtPosition(lowerNeighbourPosition))
                return false;

            var item = map.GetObjectAtPosition(lowerNeighbourPosition);
            if (item.layer != 7)
                return false;

            return true;
        }
    }
}
