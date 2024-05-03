using Assets.Core.GameEditor.DTOS;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scenes.GameEditor.Core.AIActions
{
    public abstract class ActionBase
    {
        internal EditorCanvas map;
        internal GameObject performer;
        internal Rigidbody2D performerRigidbody;
        internal float actionCost;

        public ActionBase(GameObject performer, float cost = 1) 
        {
            map = EditorCanvas.Instance;
            this.performer = performer;
            if (!performer.TryGetComponent(out performerRigidbody))
            {
                performerRigidbody = performer.AddComponent<Rigidbody2D>();
                performerRigidbody.isKinematic = true;
            }
            actionCost = cost;
        }

        public abstract bool IsPerforming();
        public abstract Task PerformAgentActionAsync(AgentActionDTO action);
        public abstract Task<List<GameObject>> PrintAgentActionAsync(AgentActionDTO action);
        public abstract List<AgentActionDTO> GetPossibleActions(Vector2 position);
        public abstract void PerformAction(string action);
        public abstract void FinishAction();

        public virtual bool ContainsActionCode(string code) { return false; }

        public virtual List<Vector2> GetReacheablePositions(Vector2 position)
        {
            var positions = new List<Vector2>();
            foreach (var action in GetPossibleActions(position))
            {
                positions.Add(action.EndPosition);
            }
            return positions;
        }

        public virtual List<GameObject> PrintReacheables(Vector2 startPosition) 
        {
            List<GameObject> markers = new List<GameObject>();

            var color = Random.ColorHSV();
            var reacheables = GetReacheablePositions(startPosition);
            
            foreach (var reacheable in reacheables)
                markers.Add(map.Marker.CreateMarkAtPosition(map.Marker.MarkerDotPrefab, reacheable, color));

            return markers;
        }

        internal bool IsWalkable(Vector2 position)
        {
            if (map.ContainsBlockingObjectAtPosition(position))
                return false;

            var _cellSize = map.GridLayout.cellSize;
            var lowerNeighbourPosition = map.GetCellCenterPosition(new Vector2(position.x, position.y - _cellSize.y));
            if (!map.ContainsObjectAtPosition(lowerNeighbourPosition))
                return false;

            var item = map.GetObjectAtPosition(lowerNeighbourPosition);
            if (item.layer != 7)
                return false;

            return true;
        }
    }
}
