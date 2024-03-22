using Assets.Core.GameEditor;
using Assets.Core.GameEditor.Components.Colliders;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.GameEditor.SourcePanels.Components.Colliders
{
    public class CircularCollider : ColliderController
    {
        [SerializeField] TMP_InputField CenterX;
        [SerializeField] TMP_InputField CenterY;

        [SerializeField] TMP_InputField Radius;

        public override ColliderComponent GetComponent()
        {
            var center = new Vector2(MathHelper.GetFloat(CenterX.text, 0), MathHelper.GetFloat(CenterY.text, 0));
            var radius = MathHelper.GetPositiveFloat(Radius.text, 1);
            return new CircleColliderComponent(center, radius);
        }

        public override void SetComponent(ColliderComponent data)
        {
            if(data is CircleColliderComponent)
            {
                var dto  = (CircleColliderComponent)data;
                CenterX.text = dto.Center.x.ToString();
                CenterY.text = dto.Center.y.ToString();
                Radius.text = dto.Radius.ToString();
            }

        }

        protected override void Awake()
        {
            base.Awake();
            CenterX.onEndEdit.AddListener(ChangePreview);
            CenterY.onEndEdit.AddListener(ChangePreview);
            Radius.onEndEdit.AddListener(ChangePreview);
        }

        private void ChangePreview(string newValue)
        {
            var center = new Vector2(MathHelper.GetFloat(CenterX.text, 0), MathHelper.GetFloat(CenterY.text, 0));
            var radius = MathHelper.GetPositiveFloat(Radius.text, 1);

            DrawCircle(center, radius);
        }
    }
}
