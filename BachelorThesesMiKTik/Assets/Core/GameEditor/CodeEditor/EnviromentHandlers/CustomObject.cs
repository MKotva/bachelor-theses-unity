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
        #endregion

        #region Animation

        #endregion

        #region PRIVATE
        #endregion
    }
}
