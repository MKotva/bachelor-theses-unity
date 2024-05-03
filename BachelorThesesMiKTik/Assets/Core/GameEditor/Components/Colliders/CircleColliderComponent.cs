using System;
using UnityEngine;

namespace Assets.Core.GameEditor.Components.Colliders
{
    [Serializable]
    public class CircleColliderComponent : ColliderComponent
    {
        public float Radius;
        public Vector2 Center;

        public CircleColliderComponent(Vector2 center, float radius, Vector2 scale)
            :base(scale)
        {
            Center = center;
            Radius = radius;
        }

        public override void Set(ItemData item)
        {
            base.Set(item);
            var collider = GetOrAddComponent<CircleCollider2D>(item.Prefab);
            collider.transform.position = Center;
            collider.radius = Radius * Scale.x;
            collider.enabled = true;
        }
    }
}
