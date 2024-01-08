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

        private Stack<CollisionSourcePanelController> instances;

        public override void SetComponent(ComponentDTO component)
        {
            if (component is BoxColliderDTO)
            {
                var boxCollider = (BoxColliderDTO) component;
                foreach (var collider in boxCollider.Colliders)
                {
                    var panel = AddSourcePanel();
                    panel.Set(collider);
                    instances.Push(panel);
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
            instances.Push(AddSourcePanel());
        }

        public void OnRemove()
        {
            var instace = instances.Pop();
            Destroy(instace.gameObject);
        }

        #region PRIVATE
        private void Awake()
        {
            instances = new Stack<CollisionSourcePanelController>();
        }

        private CollisionSourcePanelController AddSourcePanel()
        {
            return Instantiate(CollisionSourcePanel, ContentView.transform).GetComponent<CollisionSourcePanelController>();
        }

        private BoxColliderDTO CreateComponent()
        {
            var xSize = MathHelper.GetPositiveFloat(XInputField.text, "X size");
            var ySize = MathHelper.GetPositiveFloat(YInputField.text, "Y size");

            var colliders = new List<CollisionDTO>();
            foreach (var instance in instances)
            {
                colliders.Add(instance.Get());
            }

            return new BoxColliderDTO(xSize, ySize, colliders);
        }
        #endregion
    }
}
