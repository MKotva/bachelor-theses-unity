using Assets.Scripts.GameEditor.ObjectInstancesController;
using System;
using UnityEngine;

namespace Assets.Core.GameEditor.Components
{
    [Serializable]
    public class PhysicsComponent : CustomComponent
    {
        public float Mass;
        public float Gravity;
        public float LinearDrag;
        public float AngularDrag;

        public bool IsZRotationFreeze;
        public bool IsYPositionFreeze;
        public bool IsXPositionFreeze;

        public PhysicsComponent() 
        {
            ComponentName = "Physics";
            Mass = 1.0f;
            Gravity = 1.0f;
            AngularDrag = 0.05f;
            LinearDrag = 0;
        }

        public PhysicsComponent(float mass, float gravity, float linearDrag, float angularDrag, bool isZRotationFreeze, bool isYPositionFreeze, bool isXPositionFreeze)
        {
            ComponentName = "Physics";
            Mass = mass;
            Gravity = gravity;
            LinearDrag = linearDrag;
            AngularDrag = angularDrag;
            IsZRotationFreeze = isZRotationFreeze;
            IsYPositionFreeze = isYPositionFreeze;
            IsXPositionFreeze = isXPositionFreeze;
        }

        public override void Set(ItemData item)
        {
            var rigid = GetOrAddComponent<Rigidbody2D>(item.Prefab);

            foreach (var component in item.Components)
            {
                if (component.ComponentName == "Player Control" || component.ComponentName == "AI Control")
                {
                    rigid.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
                    return;
                }
            }
        }

        public override void SetInstance(ItemData item, GameObject instance)
        {
            var controller = GetOrAddComponent<PhysicsController>(instance);
            controller.Initialize(this);
        }
    }
}
