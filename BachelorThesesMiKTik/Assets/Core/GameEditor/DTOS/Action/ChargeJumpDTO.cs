﻿using Assets.Core.GameEditor.AIActions;
using Assets.Scenes.GameEditor.Core.AIActions;
using System;
using System.Collections.Generic;
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
        public float JumpSpeedCap;

        public bool OnlyGrounded;

        public ChargeJumpDTO(float vMin, float vMax, float hMin, float hMax, float timeCap, float speedCap, bool onlyGrounded)
        {
            VerticalForceMin = vMin;
            VerticalForceMax = vMax;
            HorizontalForceMin = hMin;
            HorizontalForceMax = hMax;
            JumpTimeCap = timeCap;
            JumpSpeedCap = speedCap;
            OnlyGrounded = onlyGrounded;
        }
        public override List<ActionBase> GetAction(GameObject instance)
        {
            return new List<ActionBase> {
                new ChargeableJumpAction(
                        instance,
                        this
                    )};
        }
    }
}
