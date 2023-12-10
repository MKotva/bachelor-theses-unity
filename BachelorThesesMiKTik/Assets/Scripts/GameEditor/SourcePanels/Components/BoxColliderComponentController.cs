using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.GameEditor.SourcePanels.Components
{
    public class BoxColliderComponentController : ObjectComponent
    {
        [SerializeField] TMP_InputField XInputField;
        [SerializeField] TMP_InputField YInputField;

        public override async Task SetItem(ItemData item)
        {
            if(item.Prefab.TryGetComponent(out BoxCollider2D collider))
            {
                collider.enabled = true;
                SetColliderSize(collider);
            }
        }

        private void SetColliderSize(BoxCollider2D collider)
        { 
            var xSize = GetInput(XInputField);
            var ySize = GetInput(YInputField);

            if (xSize <= 0 || ySize <= 0)
                return;

            collider.size = new Vector2(xSize, ySize);
        }

        private float GetInput(TMP_InputField field)
        {
            double size = 0;
            if (field.text != String.Empty)
            {
                size = Convert.ToDouble(field.text);
            }
            return (float)size;
        }
    }
}
