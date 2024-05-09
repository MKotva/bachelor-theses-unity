using Assets.Scenes.GameEditor.Core.AIActions;
using Assets.Scripts.GameEditor.AI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Core.GameEditor.DTOS.Action
{
    [Serializable]
    public class SimpleJumpActionDTO : ActionDTO
    {
        public float VerticalForce;
        public float HorizontalForce;
        public float SpeedCap;
        public bool OnlyGrounded;

        public SimpleJumpActionDTO(float verticalForce, float horizontalForce, float speedCap, bool onlyGrounded)
        {
            VerticalForce = verticalForce;
            HorizontalForce = horizontalForce;
            SpeedCap = speedCap;
            OnlyGrounded = onlyGrounded;
        }

        public override List<ActionBase> GetAction(GameObject instance)
        {
            return new List<ActionBase> { new JumpAction(instance, this) };
        }
    }
}
