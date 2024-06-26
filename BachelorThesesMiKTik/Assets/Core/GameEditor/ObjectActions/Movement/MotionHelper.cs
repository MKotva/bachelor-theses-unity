﻿using UnityEngine;

namespace Assets.Core.GameEditor.AIActions.Movement
{
    public static class MotionHelper
    {
        /// <summary>
        /// Checks if objects stays on solid object -> is not in air.
        /// </summary>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        public static bool CheckIfStaysOnGround(GameObject gameObject)
        {
            var position = (Vector2) gameObject.transform.position;
            var collider = gameObject.GetComponent<Collider2D>();
            var colliderSize = collider.bounds.size;

            var corners = GetCorners(position, colliderSize);
            foreach (var corner in corners)
            {
                var hits = Physics2D.RaycastAll(corner, Vector2.down, colliderSize.y + colliderSize.y * 0.2f);
                foreach (var hit in hits)
                {
                    if (hit.collider.gameObject.GetInstanceID() != gameObject.GetInstanceID())
                    {
                        if(hit.collider.isTrigger == false)
                            return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Returns lower corners and center of collider, based on given collider size.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="colliderSize"></param>
        /// <returns></returns>
        private static Vector2[] GetCorners(Vector2 position, Vector2 colliderSize)
        {
            var maxPosition = position + (colliderSize / 2f);
            var minPosition = position - ( colliderSize / 2f);
            return new Vector2[]
            {
                position,
                minPosition,
                new Vector2(maxPosition.x, minPosition.y),
            };
        }
    }
}
