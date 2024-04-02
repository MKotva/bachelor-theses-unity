using Assets.Core.GameEditor.Components;
using Assets.Core.GameEditor.Components.Colliders;
using Assets.Core.GameEditor.DTOS.SourcePanels;
using Assets.Scripts.GameEditor.SourcePanels.Components.ActionsSettings;
using Assets.Scripts.GameEditor.SourcePanels.Components.Colliders;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

using BoxCollider = Assets.Scripts.GameEditor.SourcePanels.Components.Colliders.BoxCollider;

namespace Assets.Scripts.GameEditor.SourcePanels.Components
{
    public class BoxColliderComponentController : ObjectComponent
    {
        [SerializeField] TMP_Dropdown ColliderType;
        [SerializeField] BoxCollider BoxColliderMenu;
        [SerializeField] CircularCollider CircularMenu;
        [SerializeField] PolygonCollider PolygonMenu;

        [SerializeField] GameObject ContentView;
        [SerializeField] GameObject CollisionSourcePanel;


        private GameObject actualActriveMenu;
        private List<CollisionSourcePanelController> instances;

        public override void SetComponent(CustomComponent component)
        {
            if (component is ColliderComponent)
            {
                var boxCollider = (ColliderComponent) component;
                foreach (var collider in boxCollider.Colliders)
                {
                    var panel = AddSourcePanel();
                    panel.Set(collider);
                    instances.Add(panel);
                }
                
                if(component is BoxColliderComponent)
                {
                    BoxColliderMenu.SetComponent(boxCollider);
                    actualActriveMenu = BoxColliderMenu.gameObject;
                }
                else if (component is PolygonColliderComponent)
                {
                    PolygonMenu.SetComponent(boxCollider);
                    actualActriveMenu = PolygonMenu.gameObject;
                }
                else
                {
                    CircularMenu.SetComponent(boxCollider);
                    actualActriveMenu = CircularMenu.gameObject;
                }
                actualActriveMenu.SetActive(true);
            }
            else
            {
                ErrorOutputManager.Instance.ShowMessage("General component parsing error!", "ObjectCreate");
            }
        }
        public override CustomComponent GetComponent()
        {
            return CreateComponent();
        }

        public void OnAdd()
        {
            instances.Add(AddSourcePanel());
        }

        #region PRIVATE
        private void Awake()
        {
            actualActriveMenu = BoxColliderMenu.gameObject;
            instances = new List<CollisionSourcePanelController>();
            ColliderType.onValueChanged.AddListener(ChangeMenu);
            
        }

        private void ChangeMenu(int id)
        {
            actualActriveMenu.SetActive(false);
            if (id == 0)
            {
                actualActriveMenu = BoxColliderMenu.gameObject;
            }
            else if(id == 1)
            {
                actualActriveMenu = CircularMenu.gameObject;
            }
            else
            {
                actualActriveMenu = PolygonMenu.gameObject;
            }
            actualActriveMenu.SetActive(true);
        }

        private ColliderComponent CreateComponent()
        {
            var component = actualActriveMenu.GetComponent<ColliderController>().GetComponent();

            var colliders = new List<CollisionDTO>();
            foreach (var instance in instances)
            {
                var collider = instance.Get();
                if(collider != null )
                    colliders.Add(collider);
            }

            component.Colliders = colliders;

            return component;
        }

        private CollisionSourcePanelController AddSourcePanel()
        {
            var panel = Instantiate(CollisionSourcePanel, ContentView.transform);
            panel.GetComponent<SourcePanelController>().onDestroyClick += PanelDestroyHandler;
            return panel.GetComponent<CollisionSourcePanelController>();
        }

        private void PanelDestroyHandler(int id)
        {
            for (int i = 0; i < instances.Count; i++)
            {
                if (instances[i].gameObject.GetInstanceID() == id)
                {
                    Destroy(instances[i].gameObject);
                    instances.RemoveAt(i);
                }
            }
        }
        #endregion
    }
}
