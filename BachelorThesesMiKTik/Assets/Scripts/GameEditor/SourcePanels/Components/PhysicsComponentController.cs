using Assets.Core.GameEditor;
using Assets.Core.GameEditor.DTOS.Components;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameEditor.SourcePanels.Components
{
    public class PhysicsComponentController : ObjectComponent
    {
        [SerializeField] TMP_InputField MassInputField;
        [SerializeField] TMP_InputField GravityScaleInputField;
        [SerializeField] TMP_InputField LinearDragInputField;
        [SerializeField] TMP_InputField AngularDragInputField;

        [SerializeField] Toggle ZToggle;
        [SerializeField] Toggle YToggle;
        [SerializeField] Toggle XToggle;

        public override void SetComponent(ComponentDTO component)
        {
            if (component is PhysicsComponentDTO)
            {
                var physics = (PhysicsComponentDTO) component;

                MassInputField.text = physics.Mass.ToString();
                GravityScaleInputField.text = physics.Gravity.ToString();
                LinearDragInputField.text = physics.LinearDrag.ToString();
                AngularDragInputField.text = physics.AngularDrag.ToString();

                ZToggle.isOn = physics.IsZRotationFreeze;
                YToggle.isOn = physics.IsYPositionFreeze;
                XToggle.isOn = physics.IsXPositionFreeze;
            }
            else
            {
                InfoPanelController.Instance.ShowMessage("Physics component parsing error!", "ObjectCreate");
            }
        }

        public override async Task<ComponentDTO> GetComponent()
        {
            return await Task.Run(() => CreateComponent());
        }

        private PhysicsComponentDTO CreateComponent()
        {
            var mass = MathHelper.GetFloat(MassInputField.text, "Mass", "Object creator"); ;
            var gravityScale = MathHelper.GetFloat(GravityScaleInputField.text, "Gravity", "Object creator");
            var drag = MathHelper.GetFloat(LinearDragInputField.text, "Linear drag", "Object creator");
            var angularDrag = MathHelper.GetFloat(AngularDragInputField.text, "Angular drag", "Object creator"); ;
            return new PhysicsComponentDTO(mass, gravityScale, drag, angularDrag, ZToggle.isOn, YToggle.isOn, ZToggle.isOn);
        }
    }
}
