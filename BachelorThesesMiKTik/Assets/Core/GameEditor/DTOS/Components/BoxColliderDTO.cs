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
            var collisionControler = GetOrAddComponent<CollisionController>(item.Prefab);
            collisionControler.Set(item.ShownName, Colliders);

            collider.size = new Vector2(XSize, YSize);
        }
    }
}
