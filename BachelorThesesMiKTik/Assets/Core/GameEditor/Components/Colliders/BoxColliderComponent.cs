using System;
using UnityEngine;

namespace Assets.Core.GameEditor.Components.Colliders
{
    [Serializable]
    public class BoxColliderComponent : ColliderComponent
    {
        public float XSize;
        public float YSize;

        public BoxColliderComponent(float xSize, float ySize, Vector2 scale)
            : base(scale)
        {
            XSize = xSize;
            YSize = ySize;
        }

        public override void Set(ItemData item)
        {
            var collider = GetOrAddComponent<BoxCollider2D>(item.Prefab);

            var x = (XSize * Scale.x);
            var y = (YSize * Scale.y);

            collider.size = new Vector2(x, y);
            collider.enabled = true;
        }
    }
}
