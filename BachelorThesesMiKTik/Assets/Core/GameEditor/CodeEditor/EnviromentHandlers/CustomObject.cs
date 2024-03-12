using Assets.Core.GameEditor.DTOS;
using Assets.Core.GameEditor.Enums;
using Assets.Core.SimpleCompiler.Exceptions;
using Assets.Scripts.GameEditor.GameObjects.Elements;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Core.GameEditor.CodeEditor.EnviromentObjects
{
    class CustomObject : EnviromentObject
    {
        public CustomObjectController CustomController { get; set; }

        public int HP { get; set; }
        public int Score { get; set; }

        public float MaxSpeed { get; set; } = 5f;

        public override void SetInstance(GameObject instance) { }


        #region Sprite
        public void ChangeColor(float r , float g, float b)
        {
            CheckImageConditions();
            CustomController.SpriteRenderer.color = new Color(r, g, b);
        }

        public void SetImage(string source)
        {
            SetImage(source, 0, 0);
        }

        public void SetImage(string source, float xSize, float ySize)
        {
            CheckImageConditions();

            var imageSource = new SourceDTO(SourceType.Image, source);
            CustomController.SetSource(imageSource, xSize, ySize);
        }
        #endregion

        #region Animation

        #endregion

        #region PRIVATE
        #endregion
    }
}
