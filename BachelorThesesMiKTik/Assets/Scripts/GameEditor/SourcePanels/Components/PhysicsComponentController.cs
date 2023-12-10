using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public override async Task SetItem(ItemData item)
        {
            item.Prefab.AddComponent<Rigidbody2D>();
            SetRigid(item.Prefab.GetComponent<Rigidbody2D>());
        }

        private void SetRigid(Rigidbody2D rigidbody) 
        {
            if(TryLoadInputFieldData(MassInputField, out float mass))
                rigidbody.mass = mass;

            if(TryLoadInputFieldData(GravityScaleInputField, out float gravity))
                rigidbody.gravityScale = gravity;

            if(TryLoadInputFieldData(LinearDragInputField, out float linearDrag))
                rigidbody.drag = linearDrag;

            if(TryLoadInputFieldData(AngularDragInputField, out float angular))
                rigidbody.angularDrag = angular;

            if (ZToggle.isOn)
                rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;

            if (YToggle.isOn)
                rigidbody.constraints = RigidbodyConstraints2D.FreezePositionY;

            if (XToggle.isOn)
                rigidbody.constraints = RigidbodyConstraints2D.FreezePositionX;
        }

        private bool TryLoadInputFieldData(TMP_InputField field, out float value)
        {
            value = 0;
            if (field.text == String.Empty)
            {
                return false;
            }

            value = (float) Convert.ToDouble(field.text);
            return true;
        }
    }
}
