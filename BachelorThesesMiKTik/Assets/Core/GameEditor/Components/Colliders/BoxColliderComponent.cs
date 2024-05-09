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
            base.Set(item);
            var collider = GetOrAddComponent<BoxCollider2D>(item.Prefab);
            collider.isTrigger = IsTrigger;

            var x = (XSize * Scale.x);
            var y = (YSize * Scale.y);

            collider.size = new Vector2(x, y);
            collider.enabled = true;

            
            foreach(var component in item.Components)
            {
                if (component.ComponentName == "Player Control" || component.ComponentName == "AI Control")
                    return;
            }

            if(Colliders.Count != 0 || IsTrigger) 
            {
                return;
            }

            var rigid = item.Prefab.AddComponent<Rigidbody2D>();
            rigid.isKinematic = true;

            item.Prefab.AddComponent<CompositeCollider2D>();
            collider.usedByComposite = true;
        }
    }
}
