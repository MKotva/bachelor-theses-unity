using Assets.Core.GameEditor.DTOS;
using Assets.Core.GameEditor.DTOS.Action;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Core.GameEditor.AIActions
{
    public class TrajectoryCalculator
    {
        private static EditorCanvas map;

        public static Vector2 GetJumpVector(Vector2 direction, float verticalForce, float horizontalForce)
        {
            return new Vector2(direction.x * horizontalForce, direction.y * verticalForce);
        }

        /// <summary>
        /// Based on given jumper object, simulates jump trajetory by disrete steps.
        /// </summary>
        /// <param name="jumper">Jumper object</param>
        /// <param name="startPos">Initial jump position.</param>
        /// <param name="jumpDirection">Direction of jump.</param>
        /// <param name="depth">Depth of simulation.</param>
        /// <returns></returns>
        public static TrajectoryDTO GetTrajectory(JumperDTO jumper, Vector2 startPos, Vector2 jumpDirection, int depth = 100) 
        {
            var velocity = jumpDirection.magnitude / jumper.Mass * Time.fixedDeltaTime;
            var normalizedDirection = jumpDirection.normalized;
            var trajectoryPoints = GetTrajectory(jumper, startPos, normalizedDirection, velocity, depth);

            if( trajectoryPoints.Count != 0) 
            {
                return new TrajectoryDTO(trajectoryPoints, startPos, jumpDirection, trajectoryPoints.Last());
            }

            return null;
        }

        private static List<Vector2> GetTrajectory(JumperDTO jumper, Vector2 startPos, Vector2 direction, float velocity, int depth)
        {
            if(!CheckDependecies())
                return null;

            var trajectory = new List<Vector2>();
            var previousPos = startPos;

            while (trajectory.Count < depth)
            {
                Vector2 calculatedPosition = startPos + direction * velocity * trajectory.Count * jumper.TimeTick; //Move both X and Y at a constant speed per Interval
                calculatedPosition.y += Physics2D.gravity.y / 2 * Mathf.Pow(trajectory.Count * jumper.TimeTick, 2);

                if (CheckCollision(calculatedPosition, previousPos, jumper.ColliderSize, out var previousHitPosition, out var hittedObject)) //TODO: For every ai object, have list of layers.
                {
                    if(hittedObject.GetInstanceID() != jumper.Performer.GetInstanceID())
                    {
                        var hitDirection = (calculatedPosition - previousPos).normalized;
                        var newDirection = Bounce(previousHitPosition, hitDirection, out var hasLanded);

                        if (!hasLanded && newDirection != Vector2.zero)
                        {
                            velocity = AdjustVelocity(hittedObject, velocity * trajectory.Count * jumper.TimeTick);  
                            trajectory.AddRange(GetTrajectory(jumper, previousPos, newDirection, velocity, depth - 10));   
                        }
                     
                        return trajectory;
                    }
                }

                previousPos = calculatedPosition;
                trajectory.Add(calculatedPosition);
            }

            return trajectory;
        }

        private static bool CheckCollision(Vector2 position, Vector2 PreviousPostion, Vector2 colliderSize, out Vector2 preHitPositions, out GameObject hittedObject)
        {
            var actualPositionCorners = GetCorners(position, colliderSize);
            var previousPositionCorners = GetCorners(PreviousPostion, colliderSize);

            for (int i = 0; i < actualPositionCorners.Length; i++)
            {
                var centered = map.GetCellCenterPosition(actualPositionCorners[i]);
                if (map.ContainsObjectAtPosition(centered, out hittedObject))
                {
                    if (hittedObject.TryGetComponent(out Collider2D collider))
                    {
                        preHitPositions = previousPositionCorners[i];
                        return true;
                    }
                }
            }
            hittedObject = null;
            preHitPositions = Vector2.zero;
            return false;
        }

        private static Vector2[] GetCorners(Vector2 position, Vector2 colliderSize)
        {
            var maxPosition = position + colliderSize;
            var minPosition = position - colliderSize;
            return new Vector2[]
            {
                maxPosition,
                minPosition,
                new Vector2(maxPosition.x, minPosition.y),
                new Vector2(minPosition.x, maxPosition.y),
            };
        }

        private static Vector2 Bounce(Vector2 startPosition, Vector2 motionDirection, out bool isLandPossible)
        {
            isLandPossible = false;
            RaycastHit2D hit = Physics2D.Raycast(startPosition, motionDirection, 5, LayerMask.GetMask("Box"));
         
            if (hit.collider != null)
            {
                var normal = hit.normal;
                if (( hit.normal.x > 0 && hit.normal.x < 1 ) &&
                    ( hit.normal.y > 0 && hit.normal.y < 1 ))
                {
                    if (hit.normal.y > 0)
                        normal = Vector2.up;
                    else
                        normal = Vector2.down;
                }

                //This is necessary fix, because unity hit.normal, does not return 0 but value, smaller than Epsilon.
                if (hit.normal.x < float.Epsilon && hit.normal.y == 1)
                    isLandPossible = true;

                if (hit.normal == Vector2.zero)
                {
                    isLandPossible = true;
                }

                return Vector2.Reflect(motionDirection, normal).normalized;
            }
            return Vector2.zero;
        }

        private static float AdjustVelocity(GameObject hittedObject, float velocity)
        {
            BoxCollider2D collider2D;
            if (hittedObject.TryGetComponent(out collider2D))
            {
                if(collider2D.sharedMaterial != null) 
                    return velocity * collider2D.sharedMaterial.bounciness;
            }
            return velocity * 0.1f;
        }

        private static bool CheckDependecies()
        {
            if (map == null)
            {
                var mapController = EditorCanvas.Instance;
                if (mapController == null) 
                {
                    return false;
                }
                map = mapController;
            }

            return true;
        }
    }
}
