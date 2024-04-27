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
    public class ColliderComponentController : ObjectComponent
    {
        [SerializeField] TMP_Dropdown ColliderType;
        [SerializeField] BoxCollider BoxColliderMenu;
        [SerializeField] CircularCollider CircularMenu;
        [SerializeField] PolygonCollider PolygonMenu;

        [SerializeField] GameObject ContentView;
        [SerializeField] GameObject CollisionSourcePanel;

        private GameObject actualActiveMenu;
        private List<CollisionSourcePanelController> instances;


        /// <summary>
        /// Sets component panel based on given component class.
        /// </summary>
        /// <param name="component"></param>
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
                    ColliderType.value = 0;
                    BoxColliderMenu.SetComponent(boxCollider);
                }
                else if (component is PolygonColliderComponent)
                {
                    ColliderType.value = 2;
                    PolygonMenu.SetComponent(boxCollider);
                }
                else
                {
                    ColliderType.value = 1;
                    CircularMenu.SetComponent(boxCollider);
                }
            }
            else
            {
                ErrorOutputManager.Instance.ShowMessage("General component parsing error!", "ObjectCreate");
            }
        }

        /// <summary>
        /// Returns component new class from data, in component panel. 
        /// </summary>
        /// <returns></returns>
        public override CustomComponent GetComponent()
        {
            return CreateComponent();
        }

        /// <summary>
        /// Handles Add button click by adding new collision panel. 
        /// </summary>
        public void OnAdd()
        {
            instances.Add(AddSourcePanel());
        }

        #region PRIVATE
        private void Awake()
        {
            actualActiveMenu = BoxColliderMenu.gameObject;
            instances = new List<CollisionSourcePanelController>();
            ColliderType.onValueChanged.AddListener(ChangeMenu);    
        }

        /// <summary>
        /// Changes shape of collider based on menu value. This change is
        /// performed by displaying proper setting panel.
        /// </summary>
        /// <param name="id"></param>
        private void ChangeMenu(int id)
        {
            actualActiveMenu.SetActive(false);
            if (id == 0)
            {
                actualActiveMenu = BoxColliderMenu.gameObject;
            }
            else if(id == 1)
            {
                actualActiveMenu = CircularMenu.gameObject;
            }
            else
            {
                actualActiveMenu = PolygonMenu.gameObject;
            }
            actualActiveMenu.SetActive(true);
        }


        /// <summary>
        /// Creates component class based on data and shape in component panel. Also
        /// stores all data from collision panels.
        /// </summary>
        /// <returns></returns>
        private ColliderComponent CreateComponent()
        {
            var component = actualActiveMenu.GetComponent<ColliderController>().GetComponent();
            if (component == null)
                return null;

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

        /// <summary>
        /// Creates new instance of collision panel.
        /// </summary>
        /// <returns></returns>
        private CollisionSourcePanelController AddSourcePanel()
        {
            var panel = Instantiate(CollisionSourcePanel, ContentView.transform);
            panel.GetComponent<SourcePanelController>().onDestroyClick += PanelDestroyHandler;
            return panel.GetComponent<CollisionSourcePanelController>();
        }

        /// <summary>
        /// Destroys collision panel with given instance id.
        /// </summary>
        /// <param name="id"></param>
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
