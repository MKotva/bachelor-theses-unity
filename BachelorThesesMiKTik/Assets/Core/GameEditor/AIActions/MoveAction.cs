using System;
using UnityEngine;

namespace Assets.Scenes.GameEditor.Core.AIActions
{
    public class MoveAction : AIActionBase
    {
        public Vector3 Direction { get; private set; }
        public float Force { get; private set; }

        public MoveAction(Vector3 direction, float force)
        {
            Direction = direction;
            Force = force;
        }

        public override void DoAction(GameObject performer)
        {
            throw new NotImplementedException();
        }
    }
}
