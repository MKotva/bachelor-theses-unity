using Assets.Core.GameEditor.DTOS;
using Assets.Core.SimpleCompiler.Enums;
using Assets.Scenes.GameEditor.Core.AIActions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Core.GameEditor.AIActions
{
    public class FlyAIAction : AIActionBase
    {
        private static List<string> actionTypes = new List<string>
        {
            "Fly left",
            "Fly up-left",
            "Fly down-left",
            "Fly right",
            "Fly up-right",
            "Fly down-right"
        };
        public static List<string> ActionTypes
        {
            get
            {
                return new List<string>(actionTypes);
            }
        }
  

        private float speed;
        private float speedCap;

        public FlyAIAction(float speed, float speedCap)
        {
            this.speed = speed;
            this.speedCap = speedCap;
        }


        public override List<AgentActionDTO> GetPossibleActions(Vector3 position)
        {
            throw new NotImplementedException();
        }

        public override bool IsPerforming()
        {
            throw new NotImplementedException();
        }

        public override Task PerformAgentActionAsync(AgentActionDTO action)
        {
            throw new NotImplementedException();
        }

        public override Task<List<GameObject>> PrintAgentActionAsync(AgentActionDTO action)
        {
            throw new NotImplementedException();
        }

        public override void PerformAction(string action)
        {
            throw new NotImplementedException();
        }
    }
}
