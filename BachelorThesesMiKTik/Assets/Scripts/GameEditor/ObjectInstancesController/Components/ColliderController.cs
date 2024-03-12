using Assets.Core.GameEditor.DTOS.Components;
using Assets.Scripts.GameEditor.ObjectInstancesController;
using UnityEngine;

namespace Assets.Scripts.GameEditor.Entiti
{
    public class ColliderController : MonoBehaviour, IObjectController
    {
        public string Name;
        private BoxCollider2D collider;
        private BoxColliderDTO colliderSettings;

        public void Initialize(string name, BoxColliderDTO colliderSetting)
        {
            Name = name;
            colliderSettings = colliderSetting;
            collider.size = new Vector2 (colliderSettings.XSize, colliderSettings.YSize);
        }

        public void Play()
        {
            collider.enabled = true;
        }

        public void Pause()
        {
            collider.enabled = false;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.TryGetComponent<ColliderController>(out var controller))
            {
                foreach (var handler in colliderSettings.Colliders)
                {
                    if(handler.ObjectName == controller.Name) 
                    {
                        handler.Handler.Execute(gameObject);
                    }
                }
            }
        }

        private void Awake()
        {
            if(!TryGetComponent(out collider))
                collider = gameObject.AddComponent<BoxCollider2D>();
            collider.enabled = false;
        }
    }
}
