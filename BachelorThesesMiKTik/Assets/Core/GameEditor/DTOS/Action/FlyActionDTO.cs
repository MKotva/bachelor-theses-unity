﻿using Assets.Core.GameEditor.AIActions;
using Assets.Scenes.GameEditor.Core.AIActions;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Core.GameEditor.DTOS.Action
{
    [Serializable]
    public class FlyActionDTO : ActionDTO
    {
        public float Speed;
        public float SpeedCap;

        public FlyActionDTO(float speed, float speedCap) 
        {
            Speed = speed;
            SpeedCap = speedCap;
        }

        public override List<ActionBase> GetAction(GameObject instance)
        {
            return new List<ActionBase> { new FlyAction(instance, Speed, SpeedCap) };
        }
    }
}
