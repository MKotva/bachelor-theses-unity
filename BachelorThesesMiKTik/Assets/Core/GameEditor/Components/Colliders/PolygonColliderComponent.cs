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
            base.Set(item);

            var collider = GetOrAddComponent<PolygonCollider2D>(item.Prefab);
            collider.isTrigger = IsTrigger;

            var scaledPoints = new List<Vector2>();
            foreach(var point in Points)
            {
                scaledPoints.Add(new Vector2(point.x * Scale.x, point.y * Scale.y));
            }
            collider.SetPath(0, scaledPoints);
            collider.enabled = true;

            foreach (var component in item.Components)
            {
                if (component.ComponentName == "Player Control" || component.ComponentName == "AI Control")
                    return;
            }

            var rigid = item.Prefab.AddComponent<Rigidbody2D>();
            rigid.isKinematic = true;

            item.Prefab.AddComponent<CompositeCollider2D>();
            collider.usedByComposite = true;
        }
    }
}
