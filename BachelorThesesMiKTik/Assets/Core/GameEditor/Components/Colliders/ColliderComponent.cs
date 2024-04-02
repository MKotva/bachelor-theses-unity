using Assets.Core.GameEditor.DTOS.SourcePanels;
using Assets.Scripts.GameEditor.Entiti;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Core.GameEditor.Components.Colliders
{
    [Serializable]
    public class ColliderComponent : CustomComponent
    {
        public List<CollisionDTO> Colliders;

        public ColliderComponent() 
        {
            ComponentName = "Collider";
            Colliders = new List<CollisionDTO>();
        }

        public ColliderComponent(List<CollisionDTO> colliders) 
        {
            ComponentName = "Collider";
            Colliders = colliders;
        }

        public override void Set(ItemData item)
        {
            var collider = GetOrAddComponent<BoxCollider2D>(item.Prefab);
            collider.enabled = true;
        }

        public override void SetInstance(ItemData item, GameObject instance)
        {
            var collisionControler = GetOrAddComponent<ColliderController>(instance);
            collisionControler.Initialize(item.ShownName, item.GroupName, this);
        }
    }
}
