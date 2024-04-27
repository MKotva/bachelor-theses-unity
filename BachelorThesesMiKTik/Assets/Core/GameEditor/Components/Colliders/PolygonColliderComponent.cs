using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Core.GameEditor.Components.Colliders
{
    [Serializable]
    public class PolygonColliderComponent : ColliderComponent
    {
        public List<Vector2> Points;

        public PolygonColliderComponent(List<Vector2> points, Vector2 scale) : base(scale)
        {
            Points = points;
        }

        public override void Set(ItemData item)
        {
            var collider = GetOrAddComponent<PolygonCollider2D>(item.Prefab);
            
            var scaledPoints = new List<Vector2>();
            foreach(var point in Points)
            {
                scaledPoints.Add(new Vector2(point.x * Scale.x, point.y * Scale.y));
            }
            collider.SetPath(0, scaledPoints);
            collider.enabled = true;
        }
    }
}
