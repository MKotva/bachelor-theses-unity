using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Core.GameEditor.Components.Colliders
{
    [Serializable]
    public class PolygonColliderComponent : ColliderComponent
    {
        public List<Vector2> Points;

        public PolygonColliderComponent(List<Vector2> points)
        {
            Points = points;
        }

        public override void Set(ItemData item)
        {
            var collider = GetOrAddComponent<PolygonCollider2D>(item.Prefab);
            collider.SetPath(Points.Count - 1, Points);
            collider.enabled = true;
        }
    }
}
