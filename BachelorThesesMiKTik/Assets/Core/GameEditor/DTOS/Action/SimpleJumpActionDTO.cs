using Assets.Scenes.GameEditor.Core.AIActions;
using Assets.Scripts.GameEditor.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Core.GameEditor.DTOS.Action
{
    [Serializable]
    public class SimpleJumpActionDTO : ActionDTO
    {
        public float VerticalForce;
        public float HorizontalForce;
        public bool OnlyGrounded;

        public SimpleJumpActionDTO(float verticalForce, float horizontalForce, bool onlyGrounded)
        {
            VerticalForce = verticalForce;
            HorizontalForce = horizontalForce;
            OnlyGrounded = onlyGrounded;
        }

        public override List<ActionBase> GetAction(GameObject instance)
        {
            return new List<ActionBase> { new JumpAction(instance, VerticalForce, HorizontalForce, OnlyGrounded) };
        }
    }
}
