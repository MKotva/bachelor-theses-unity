using Assets.Core.GameEditor.AIActions;
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
    public class ChargeJumpDTO : ActionDTO
    {
        public float VerticalForceMin;
        public float VerticalForceMax;
        public float HorizontalForceMin;
        public float HorizontalForceMax;
        public float JumpTimeCap;

        public ChargeJumpDTO(float vMin, float vMax, float hMin, float hMax, float timeCap)
        {
            VerticalForceMin = vMin;
            VerticalForceMax = vMax;
            HorizontalForceMin = hMin;
            HorizontalForceMax = hMax;
            JumpTimeCap = timeCap;
        }
        public override List<AIActionBase> GetAction(GameObject instance)
        {
            return new List<AIActionBase> { 
                new ChargeableJumpAIAction(
                        instance,
                        VerticalForceMin,
                        VerticalForceMax,
                        HorizontalForceMin,
                        HorizontalForceMax,
                        JumpTimeCap
                    )};
        }
    }
}
