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
    public class MoveActionDTO : ActionDTO
    {
        public float Speed;
        public float SpeedCap;
        public bool CanFall;

        public MoveActionDTO(float speed, float cap, bool canFall) 
        {
            Speed = speed;
            SpeedCap = cap;
            CanFall = canFall;
        }

        public override List<ActionBase> GetAction(GameObject instance)
        {
            return new List<ActionBase> { new MoveAction(instance, Speed, SpeedCap, CanFall) };
        }
    }
}
