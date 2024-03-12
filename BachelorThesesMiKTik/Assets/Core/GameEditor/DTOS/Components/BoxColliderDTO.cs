using Assets.Core.GameEditor.DTOS.SourcePanels;
using Assets.Scripts.GameEditor.Entiti;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Core.GameEditor.DTOS.Components
{
    [Serializable]
    public class BoxColliderDTO : ComponentDTO
    {
        public float XSize;
        public float YSize;
        public List<CollisionDTO> Colliders;

        public BoxColliderDTO() 
        {
            XSize = 30;
            YSize = 30;
            Colliders = new List<CollisionDTO>();
        }

        public BoxColliderDTO(float xSize, float ySize, List<CollisionDTO> colliders) 
        {
            ComponentName = "Collider";
            XSize = xSize;
            YSize = ySize;
            Colliders = colliders;
        }

        public override async Task Set(ItemData item)
        {
            var collider = GetOrAddComponent<BoxCollider2D>(item.Prefab);
            collider.enabled = true;
            collider.size = new Vector2(XSize, YSize);
        }

        public override void SetInstance(ItemData item, GameObject instance)
        {
            var collisionControler = GetOrAddComponent<ColliderController>(item.Prefab);
            collisionControler.Initialize(item.ShownName, this);
        }
    }
}
