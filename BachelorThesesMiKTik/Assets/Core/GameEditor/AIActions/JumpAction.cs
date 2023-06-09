using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scenes.GameEditor.Core.AIActions
{
    public class JumpAction : AIActionBase
    {
        private Vector3 JumpDirection;
        private float JumpForce;

        public JumpAction(Vector3 jumpDir, float force) 
        {
            JumpDirection = jumpDir;
            JumpForce = force;
        }

        public override void DoAction(GameObject performer)
        {
            throw new NotImplementedException();
        }
    }
}
