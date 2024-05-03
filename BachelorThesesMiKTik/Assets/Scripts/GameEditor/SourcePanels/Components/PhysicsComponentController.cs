using Assets.Core.GameEditor;
using Assets.Core.GameEditor.Components;
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

        public override void SetComponent(CustomComponent component)
        {
            if (component is PhysicsComponent)
            {
                var physics = (PhysicsComponent) component;

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
                OutputManager.Instance.ShowMessage("Physics component parsing error!", "ObjectCreate");
            }
        }

        public override CustomComponent GetComponent()
        {
            return CreateComponent();
        }

        private PhysicsComponent CreateComponent()
        {
            var mass = MathHelper.GetFloat(MassInputField.text, 1, "Mass", "Object creator"); ;
            var gravityScale = MathHelper.GetFloat(GravityScaleInputField.text, 1, "Gravity", "Object creator");
            var drag = MathHelper.GetFloat(LinearDragInputField.text, 0, "Linear drag", "Object creator");
            var angularDrag = MathHelper.GetFloat(AngularDragInputField.text, 0.05f,"Angular drag", "Object creator"); ;
            return new PhysicsComponent(mass, gravityScale, drag, angularDrag, ZToggle.isOn, YToggle.isOn, XToggle.isOn);
        }
    }
}
