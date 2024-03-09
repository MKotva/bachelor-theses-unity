using Assets.Core.GameEditor;
using Assets.Core.GameEditor.DTOS.Components;
using Assets.Core.GameEditor.DTOS.SourcePanels;
using Assets.Scripts.GameEditor.SourcePanels.Components.ActionsSettings;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.GameEditor.SourcePanels.Components
{
    public class BoxColliderComponentController : ObjectComponent
    {
        [SerializeField] TMP_InputField XInputField;
        [SerializeField] TMP_InputField YInputField;
        [SerializeField] GameObject ContentView;
        [SerializeField] GameObject CollisionSourcePanel;

        private List<CollisionSourcePanelController> instances;

        public override void SetComponent(ComponentDTO component)
        {
            if (component is BoxColliderDTO)
            {
                var boxCollider = (BoxColliderDTO) component;
                foreach (var collider in boxCollider.Colliders)
                {
                    var panel = AddSourcePanel();
                    panel.Set(collider);
                    instances.Add(panel);
                }
                XInputField.text = boxCollider.XSize.ToString();
                YInputField.text = boxCollider.YSize.ToString();
            }
            else
            {
                InfoPanelController.Instance.ShowMessage("General component parsing error!");
            }
        }
        public override async Task<ComponentDTO> GetComponent()
        {
            return await Task.Run(() => CreateComponent());
        }

        public void OnAdd()
        {
            instances.Add(AddSourcePanel());
        }

        #region PRIVATE
        private void Awake()
        {
            instances = new List<CollisionSourcePanelController>();
        }

        private BoxColliderDTO CreateComponent()
        {
            var xSize = MathHelper.GetPositiveFloat(XInputField.text, "X size", "Object creator");
            var ySize = MathHelper.GetPositiveFloat(YInputField.text, "Y size", "Object creator");

            var colliders = new List<CollisionDTO>();
            foreach (var instance in instances)
            {
                colliders.Add(instance.Get());
            }

            return new BoxColliderDTO(xSize, ySize, colliders);
        }

        private CollisionSourcePanelController AddSourcePanel()
        {
            var panel = Instantiate(CollisionSourcePanel, ContentView.transform);
            panel.GetComponent<SourcePanelController>().onDestroy += PanelDestroyHandler;
            return panel.GetComponent<CollisionSourcePanelController>();
        }

        private void PanelDestroyHandler(int id)
        {
            for (int i = 0; i < instances.Count; i++)
            {
                if (instances[i].GetInstanceID() == id)
                {
                    instances.RemoveAt(i);
                }
            }
        }
        #endregion
    }
}
