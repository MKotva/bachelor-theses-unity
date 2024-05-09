using Assets.Core.GameEditor.DTOS;
using Assets.Scripts.GameEditor.Entiti;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scenes.GameEditor.Core.AIActions
{
    public class ActionBase
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

        public virtual bool IsPerforming() { return false; }
        public virtual bool PerformAgentAction(AgentActionDTO action, Queue<AgentActionDTO> actions, float deltaTime) { return true; }
        public virtual List<GameObject> PrintAgentAction(AgentActionDTO action) { return null; }
        public virtual List<AgentActionDTO> GetPossibleActions(Vector2 position) { return null; }
        public virtual void PerformAction(string action) { }
        public virtual void FinishAction() { }
        public virtual AgentActionDTO GetRandomAction(Vector2 lastPosition) { return null; }
        public virtual List<Vector3> GetReacheablePositions(Vector2 position) { return null; }
        public virtual List<GameObject> PrintReacheables(Vector2 startPosition) { return null; }
        public virtual void ClearAction() { }
        public virtual void ClampSpeed() { }

        public virtual bool ContainsActionCode(string code) { return false; }
        protected virtual bool IsWalkable(Vector2 position)
        {
            if (ContainsBlockingObjectAtPosition(position))
            {
                return false;
            }

            var _cellSize = map.GridLayout.cellSize;
            var lowerNeighbourPosition = new Vector2(position.x, position.y - _cellSize.y);
            if (!ContainsBlockingObjectAtPosition(lowerNeighbourPosition))
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
                if (!performer.TryGetComponent(out colliderController))
                    return false;
            }

            var centeredPosition = map.GetCellCenterPosition(position);
            if (map.ContainsObjectAtPosition(centeredPosition, out GameObject ob))
            {
                if (ob.layer == 11)
                    return false;

                if (ob.TryGetComponent(out ColliderController controller))
                {
                    if (controller == null || controller.ObjectCollider == null)
                    {
                        return true;
                    }

                    if (!controller.ObjectCollider.isTrigger)
                    {
                        return true;
                    }

                    //if(controller.ContainsHandler(colliderController.Name))
                    //{
                    //    return true;
                    //}
                }
            }
            return false;
        }


        protected void ActivateObject()
        {
            if (performerRigidbody == null)
                return;

            performerRigidbody.isKinematic = false;
            performerRigidbody.WakeUp();

            if(colliderController != null)
            {
                colliderController.Play();
            }
        }

        protected void DeactivateObject()
        {
            if (performerRigidbody == null)
                return;

            performerRigidbody.isKinematic = true;
            performerRigidbody.Sleep();

            if (colliderController != null)
            {
                colliderController.Pause();
            }
        }
    }
}
