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

        public SimpleJumpActionDTO(float verticalForce, float horizontalForce)
        {
            VerticalForce = verticalForce;
            HorizontalForce = horizontalForce;
        }

        public override List<AIActionBase> GetAction(GameObject instance)
        {
            return new List<AIActionBase> { new JumpAIAction(instance, VerticalForce, HorizontalForce) };
        }
    }
}
