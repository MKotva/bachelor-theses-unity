using Assets.Core.GameEditor.DTOS;
using Assets.Scenes.GameEditor.Core.AIActions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Core.GameEditor.AIActions
{
    public class FlyAction : ActionBase
    {
        private static Dictionary<string, Vector2> actionTypes = new Dictionary<string, Vector2>
        {
            {"Fly up", Vector2.up},
            {"Fly up-right", new Vector2(1, 1)},
            {"Fly right", Vector2.right},
            {"Fly down-right", new Vector2(1, -1)},
            {"Fly down", Vector2.down},
            {"Fly down-left", new Vector2(-1, -1)},
            {"Fly left", Vector2.left},
            {"Fly up-left", new Vector2(-1, 1)}
        };
        public static List<string> ActionTypes
        {
            get
            {
                return actionTypes.Keys.ToList();
            }
        }

        private float speed;
        private float speedCap;

        public FlyAction(GameObject performer, float speed, float speedCap) : base(performer)
        {
            this.speed = speed;
            this.speedCap = speedCap;
        }


        public override List<AgentActionDTO> GetPossibleActions(Vector2 position)
        {
            var reacheablePositions = new List<AgentActionDTO>();

            foreach (var actionKey in actionTypes.Keys)
            {
                var translation = GetPositionFromParam(actionKey);
                var translatedPosition = new Vector2
                {
                    x = translation.x + position.x,
                    y = translation.y + position.y
                };

                var centeredPositon = map.GetCellCenterPosition(translatedPosition);
                if (IsWalkable(centeredPositon))
                {
                    reacheablePositions.Add(new AgentActionDTO(position, centeredPositon, actionKey, 1f, PerformAgentActionAsync, PrintAgentActionAsync));
                }
            }

            return reacheablePositions;
        }

        public override async Task PerformAgentActionAsync(AgentActionDTO action)
        {
            var position = performer.transform.position;
            var translation = GetPositionFromParam(action.ActionParameters);
            performer.transform.position = new Vector2
            {
                x = translation.x + position.x,
                y = translation.y + position.y
            };

            await Task.Delay(1000);
        }

        public override async Task<List<GameObject>> PrintAgentActionAsync(AgentActionDTO action)
        {
            var result = new List<GameObject>() { map.Marker.CreateMarkAtPosition(action.StartPosition) };
            return await Task.FromResult(result);
        }

        public override void PerformAction(string action)
        {
            if (!actionTypes.ContainsKey(action))
                return;

            var direction = actionTypes[action];
            performerRigidbody.AddForce(direction * speed);
            performerRigidbody.velocity = Vector3.ClampMagnitude(performerRigidbody.velocity, speedCap);
        }

        public override void FinishAction() { }

        public override bool IsPerforming()
        {
            throw new NotImplementedException();
        }

        public override bool ContainsActionCode(string code)
        {
            return ActionTypes.Contains(code);
        }

        private Vector2 GetPositionFromParam(string action) 
        {
            if (!actionTypes.ContainsKey(action))
            {
                return Vector2.zero;
            }

            var cellSize = map.GridLayout.cellSize;
            var direction = actionTypes[action];
            return new Vector2(cellSize.x * direction.x, cellSize.y * direction.y);
        }
    }
}
