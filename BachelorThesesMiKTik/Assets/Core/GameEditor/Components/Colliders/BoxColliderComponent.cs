using System;
using UnityEngine;

namespace Assets.Core.GameEditor.Components.Colliders
{
    [Serializable]
    public class BoxColliderComponent : ColliderComponent
    {
        public float XSize;
        public float YSize;

        public BoxColliderComponent(float xSize, float ySize)
        {
            XSize = xSize;
            YSize = ySize;
        }

        public override void Set(ItemData item)
        {
            var collider = GetOrAddComponent<BoxCollider2D>(item.Prefab);
            collider.size = new Vector2(XSize, YSize);
            collider.enabled = true;
        }
    }
}
