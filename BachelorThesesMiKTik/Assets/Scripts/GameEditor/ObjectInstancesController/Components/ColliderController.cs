using Assets.Core.GameEditor.Components.Colliders;
using Assets.Scripts.GameEditor.ObjectInstancesController;
using UnityEngine;

namespace Assets.Scripts.GameEditor.Entiti
{
    public class ColliderController : MonoBehaviour, IObjectController
    {
        public string Name;
        public string GroupName;

        private Collider2D objectCollider;
        private ColliderComponent colliderSettings;

        public void Initialize(string name, string groupName, ColliderComponent colliderSetting)
        {
            Name = name;
            GroupName = groupName;
            colliderSettings = colliderSetting;
        }

        #region IObjectMethods
        public void Play()
        {
            objectCollider.enabled = true;
        }

        public void Pause()
        {
            objectCollider.enabled = false;
        }

        public void Enter() {}

        public void Exit() 
        {
            objectCollider.enabled = false;
        }

        #endregion

        //TODO: Consider construction of data structure for faster name check: MB: List<Dictionary<string, Code>>
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.TryGetComponent<ColliderController>(out var controller))
            {
                foreach (var handler in colliderSettings.Colliders)
                {
                    if(handler.ObjectsNames.Contains(controller.Name)) 
                    {
                        handler.Handler.Execute(gameObject);
                    }
                    else if(handler.GroupsNames.Contains(controller.GroupName))
                    {
                        handler.Handler.Execute(gameObject);
                    }
                }
            }
        }

        private void Awake()
        {
            colliderSettings = new ColliderComponent();

            if (TryGetComponent<ObjectController>(out var controller))
            {
                controller.Components.Add(typeof(ColliderController), this);
                if (!TryGetComponent(out objectCollider))
                    objectCollider = gameObject.AddComponent<BoxCollider2D>();
                objectCollider.enabled = false;
            }
        }
    }
}
