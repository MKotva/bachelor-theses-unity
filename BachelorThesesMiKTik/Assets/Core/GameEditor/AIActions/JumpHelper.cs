using Assets.Core.GameEditor.DTOS;
using Assets.Core.GameEditor.DTOS.Action;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Core.GameEditor.AIActions
{
    public class JumpHelper
    {
        private static MapCanvas map;

        public static bool CheckIfStaysOnGround(GameObject gameObject)
        {
             var position = (Vector2)gameObject.transform.position;
            var collider = gameObject.GetComponent<Collider2D>();
            var colliderSize = collider.bounds.size.y;
     
            var hits = Physics2D.RaycastAll(position, Vector2.down, colliderSize);
            foreach ( var hit in hits ) 
            {
                if(hit.collider.gameObject.GetInstanceID() != gameObject.GetInstanceID())
                    return true;
            }
            return false;
        }

        public static Vector2 GetJumpVector(Vector2 direction, float verticalForce, float horizontalForce)
        {
            return ( Vector2.up * verticalForce ) + ( direction * horizontalForce );
        }

        public static TrajectoryDTO GetTrajectory(JumperDTO jumper, Vector2 startPos, Vector2 jumpDirection, int depth = 40)
        {
            if(!CheckDependecies())
                return null;

            var trajectoryPoints = new List<Vector3>();

            var actualDirection = jumpDirection;
            var position = startPos;
            var previousPos = position;
            var previousDir = actualDirection;

            while (trajectoryPoints.Count < depth)
            {
                actualDirection = (actualDirection + jumper.GravityAcceleration) * jumper.Drag;
                position = MathHelper.Add(position, actualDirection);

                Vector2 previousHitPosition;
                GameObject hittedObject;
                if (CheckCollision(position, previousPos, jumper.ColliderSize, out previousHitPosition, out hittedObject)) //TODO: For every ai object, have list of layers.
                {
                    bool hasLanded;
                    actualDirection = HandleCollision(hittedObject, previousHitPosition, previousDir, out hasLanded);
                    if (hasLanded)
                        return new TrajectoryDTO(trajectoryPoints, startPos, jumpDirection, map.GetCellCenterPosition(trajectoryPoints.Last()));
                }

                trajectoryPoints.Add(position);

                previousDir = actualDirection;
                previousPos = position;
            }
            return new TrajectoryDTO(trajectoryPoints, startPos, jumpDirection, map.GetCellCenterPosition(trajectoryPoints.Last()));
        }

        private static bool CheckCollision(Vector2 position, Vector2 PreviousPostion, Vector2 colliderSize, out Vector2 preHitPositions, out GameObject hittedObject)
        {
            var actualPositionCorners = GetCorners(position, colliderSize);
            var previousPositionCorners = GetCorners(PreviousPostion, colliderSize);

            for (int i = 0; i < actualPositionCorners.Length; i++)
            {
                var centered = map.GetCellCenterPosition(actualPositionCorners[i]);
                if (map.ContainsObjectAtPosition(centered, new int[] { 7, 8 })) //TODO: For every ai object, have list of layers.
                {
                    hittedObject = map.GetObjectAtPosition(centered);
                    preHitPositions = previousPositionCorners[i];
                    return true;
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
                new Vector2(minPosition.y, maxPosition.x),
            };
        }

        private static Vector2 HandleCollision(GameObject hittedObject, Vector2 startPos, Vector2 motionDirection, out bool isLandingPossible)
        {
            var bounceDirection = Bounce(startPos, motionDirection, out isLandingPossible);

            BoxCollider2D collider2D;
            if (hittedObject.TryGetComponent(out collider2D))
            {
                if (!isLandingPossible)
                {
                    bounceDirection *= collider2D.sharedMaterial.bounciness + 1.1f;
                    isLandingPossible = false;
                }
            }
            else
            {
                bounceDirection *= 0.1f; //Default bounce force.
                isLandingPossible = false;
            }
            return bounceDirection;
        }

        private static Vector2 Bounce(Vector2 startPosition, Vector2 motionDirection, out bool isLandPossible)
        {
            isLandPossible = false;
            RaycastHit2D hit = Physics2D.Raycast(startPosition, motionDirection, 1, LayerMask.GetMask("Box"));
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

                if (hit.normal == Vector2.up)
                    isLandPossible = true;

                if (hit.normal == Vector2.zero)
                {
                    isLandPossible = true;
                }

                return Vector2.Reflect(motionDirection, normal);
            }
            return Vector2.zero;
        }

        private static bool CheckDependecies()
        {
            if (map == null)
            {
                var mapController = MapCanvas.Instance;
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
