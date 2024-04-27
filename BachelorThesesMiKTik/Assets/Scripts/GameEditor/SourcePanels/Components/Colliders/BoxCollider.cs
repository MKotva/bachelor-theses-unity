using Assets.Core.GameEditor;
using Assets.Core.GameEditor.Components.Colliders;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.GameEditor.SourcePanels.Components.Colliders
{
    public class BoxCollider : ColliderController
    {
        [SerializeField] TMP_InputField SizeX;
        [SerializeField] TMP_InputField SizeY;

        public override ColliderComponent GetComponent()
        {
            var xSize = MathHelper.GetFloat(SizeX.text, 1);
            var ySize = MathHelper.GetFloat(SizeY.text, 1);

            return new BoxColliderComponent(xSize, ySize, counterScale);
        }

        public override void SetComponent(ColliderComponent data)
        {
            if (data is BoxColliderComponent)
            {
                var componentData = (BoxColliderComponent)data;
                SizeX.text = componentData.XSize.ToString();
                SizeY.text = componentData.YSize.ToString();
            }
        }

        protected override void Awake()
        {
            base.Awake();
            SizeX.onEndEdit.AddListener(ChangePreview);
            SizeY.onEndEdit.AddListener(ChangePreview);
        }

        private void ChangePreview(string change)
        {
            var xPos = MathHelper.GetFloat(SizeX.text, 1) / 2;
            var yPos = MathHelper.GetFloat (SizeY.text, 1) / 2;

            var upperRight = new Vector2(xPos, yPos);
            var lowerRight = new Vector2(xPos, -yPos);
            var loverLeft = new Vector2(-xPos, -yPos);
            var upperLeft = new Vector2(-xPos, yPos);

            var lines = new List<(Vector2, Vector2)>() 
            { 
                (upperRight, lowerRight),
                (lowerRight, loverLeft),
                (loverLeft, upperLeft),
                (upperLeft, upperRight)
            };

            DrawLines(lines);
        }
    }
}
