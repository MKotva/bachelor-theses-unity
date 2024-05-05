using Assets.Core.GameEditor.DTOS.SourcePanels;
using Assets.Scripts.GameEditor.Entiti;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Core.GameEditor.Components.Colliders
{
    [Serializable]
    public class ColliderComponent : CustomComponent
    {
        public List<CollisionDTO> Colliders;
        public Vector2 Scale;
        public bool IsTrigger;

        public ColliderComponent() 
        {
            ComponentName = "Collider";
            Colliders = new List<CollisionDTO>();
            Scale = new Vector2(1f / 100f, 1f / 100f);
            IsTrigger = false;
        }

        public ColliderComponent(Vector2 scale)
        {
            ComponentName = "Collider";
            Colliders = new List<CollisionDTO>();
            Scale = scale;
            IsTrigger = false;
        }

        public ColliderComponent(List<CollisionDTO> colliders, Vector2 scale) 
        {
            ComponentName = "Collider";
            Colliders = colliders;
            Scale = scale;
            IsTrigger = false;
        }

        public override async Task Initialize()
        {
            var tasks = new List<Task>();
            foreach (var collider in Colliders) 
            {
                if(collider.Handler != null)
                    tasks.Add(collider.Handler.CompileAsync());
            }
            await Task.WhenAll(tasks);
        }

        public override void Set(ItemData item) 
        {
            item.Prefab.layer = LayerMask.NameToLayer("Box");
        }

        public override void SetInstance(ItemData item, GameObject instance)
        {
            var collisionControler = GetOrAddComponent<ColliderController>(instance);
            collisionControler.Initialize(item.ShownName, item.GroupName, this);
        }
    }
}
