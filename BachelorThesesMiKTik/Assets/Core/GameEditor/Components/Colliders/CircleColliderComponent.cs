using System;
using UnityEngine;

namespace Assets.Core.GameEditor.Components.Colliders
{
    [Serializable]
    public class CircleColliderComponent : ColliderComponent
    {
        public Vector2 Center;
        public float Radius;

        public CircleColliderComponent(Vector2 center, float radius)
        {
            Center = center;
            Radius = radius;
        }

        public override void Set(ItemData item)
        {
            var collider = GetOrAddComponent<CircleCollider2D>(item.Prefab);
            collider.transform.position = Center;
            collider.radius = Radius;
            collider.enabled = true;
        }
    }
}
