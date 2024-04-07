﻿using Assets.Core.GameEditor.Attributes;
using Assets.Core.SimpleCompiler.Exceptions;
using Assets.Scripts.GameEditor.ObjectInstancesController;
using UnityEngine;

namespace Assets.Core.GameEditor.CodeEditor.EnviromentObjects
{
    class CustomObject : EnviromentObject
    {
        private Rigidbody2D rigid;
        private ObjectController controller;

        public int HP { get; set; }
        public int Score { get; set; }

        public override bool SetInstance(GameObject instance) 
        { 
            controller = instance.GetComponent<ObjectController>();
            if (controller == null) 
            {
                return false;
            }

            instance.TryGetComponent(out rigid);
            return true; 
        }

        [CodeEditorAttribute("If players velocity is aproximately equal to zero, returns true. Otherwise returns false.")]
        public bool CheckIfIsMoving()
        {
            if (rigid == null)
                throw new RuntimeException($"\"Exception in method \\\"CheckIfIsMoving\\\"! Object is missing Physics Component!");

            return false;
        }

        [CodeEditorAttribute("Destroyes actual object. If the actual animation or audio clip should be finished before" +
            "object's destruction, set the bool values to true.", "(bool finishAnimation, bool finishAudio)")]
        public void Kill(bool finishAnimation, bool finishAudio)
        {
            controller.Kill(finishAnimation, finishAudio);
        }

        [CodeEditorAttribute("Freezes actual object -> ")]
        public void Freeze()
        {
            controller.Pause();
        }

        [CodeEditorAttribute("Finds created animation by given name and sets it for this object.")]
        public void UnFreeze()
        {
            controller.Play();
        }
    }
}
