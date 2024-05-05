using Assets.Core.GameEditor.DTOS;
using Assets.Scripts.GameEditor.Entiti;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scenes.GameEditor.Core.AIActions
{
    public abstract class ActionBase
    {
        internal System.Random random;
        internal EditorCanvas map;
        internal GameObject performer;
        internal Rigidbody2D performerRigidbody;
        internal ColliderController colliderController;
        internal float actionCost;

        public ActionBase(GameObject performer, float cost = 1) 
        {
            random = new System.Random();
            map = EditorCanvas.Instance;
            this.performer = performer;
            if (!performer.TryGetComponent(out performerRigidbody))
            {
                performerRigidbody = performer.AddComponent<Rigidbody2D>();
                performerRigidbody.isKinematic = true;
            }
            performer.TryGetComponent(out colliderController);

            actionCost = cost;
        }

        public abstract bool IsPerforming();
        public abstract bool PerformAgentActionAsync(AgentActionDTO action, Queue<AgentActionDTO> actions, float deltaTime);
        public abstract Task<List<GameObject>> PrintAgentActionAsync(AgentActionDTO action);
        public abstract List<AgentActionDTO> GetPossibleActions(Vector2 position);
        public abstract void PerformAction(string action);
        public abstract AgentActionDTO GetRandomAction(Vector2 lastPosition);
        public abstract void FinishAction();
        public virtual void ClearAction() { }

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

        protected virtual bool IsWalkable(Vector2 position)
        {
            if (ContainsBlockingObjectAtPosition(position))
            {
                return false;
            }

            var _cellSize = map.GridLayout.cellSize;
            var lowerNeighbourPosition = new Vector2(position.x, position.y - _cellSize.y);
            if(!ContainsBlockingObjectAtPosition(lowerNeighbourPosition))
            {
                return false;
            }

            return true;
        }

        //TODO: Rework this, make with layers.
        protected bool ContainsBlockingObjectAtPosition(Vector2 position)
        {
            if (colliderController == null)
            {
                if(!performer.TryGetComponent(out colliderController))
                    return false;
            }

            var centeredPosition = map.GetCellCenterPosition(position);
            if (map.ContainsObjectAtPosition(centeredPosition, out GameObject ob))
            {
                if(ob.layer == 11)
                    return false;

                if (ob.TryGetComponent(out ColliderController controller))
                { 
                    if(controller == null || controller.ObjectCollider == null) 
                    {
                        return true;
                    }

                    if(!controller.ObjectCollider.isTrigger)
                    {
                        return true;
                    }

                    if(controller.ContainsHandler(colliderController.Name))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
