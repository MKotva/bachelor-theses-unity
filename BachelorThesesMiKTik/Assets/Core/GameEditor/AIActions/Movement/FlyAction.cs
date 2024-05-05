using Assets.Core.GameEditor.AIActions.Movement;
using Assets.Core.GameEditor.DTOS;
using Assets.Scenes.GameEditor.Core.AIActions;
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
            {"Fly right", Vector2.right},
            {"Fly down", Vector2.down},
            {"Fly left", Vector2.left},
            {"Fly down-left", new Vector2(-1, -1)},
            {"Fly down-right", new Vector2(1, -1)},
            {"Fly up-left", new Vector2(-1, 1)},
            {"Fly up-right", new Vector2(1, 1)}
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
        private LinearTranslator moveHelper;

        public FlyAction(GameObject performer, float speed, float speedCap) : base(performer)
        {
            this.speed = speed;
            this.speedCap = speedCap;
        }


        public override List<AgentActionDTO> GetPossibleActions(Vector2 position)
        {
            var reacheablePositions = new List<AgentActionDTO>();

            var keys = actionTypes.Keys.ToList();
            for (int i = 0; i < 4; i++ )
            {
                var translation = GetPositionFromParam(keys[i]);
                var translatedPosition = new Vector2
                {
                    x = translation.x + position.x,
                    y = translation.y + position.y
                };

                var centeredPositon = map.GetCellCenterPosition(translatedPosition);
                if (!ContainsBlockingObjectAtPosition(centeredPositon))
                {
                    reacheablePositions.Add(new AgentActionDTO(position, centeredPositon, keys[i], 1f, PerformAgentActionAsync, PrintAgentActionAsync));
                }
            }

            return reacheablePositions;
        }

        public override bool PerformAgentActionAsync(AgentActionDTO action, Queue<AgentActionDTO> actions, float deltaTime)
        {
            if(moveHelper == null)
            {
                moveHelper = new LinearTranslator(performerRigidbody, speed, action.StartPosition, map.GetCellCenterPosition(LinearTranslator.FindContinuousPath(action, actions)));
            }

            if(!moveHelper.TranslationTick(performer, map, deltaTime))
            {
                return false;
            }

            moveHelper = null;
            return true;
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

        public override AgentActionDTO GetRandomAction(Vector2 lastPosition)
        {
            var initActions = GetPossibleActions(lastPosition);
            if (initActions.Count == 0)
                return null;

            var action = initActions[random.Next(0, initActions.Count)];

            var newAction = action;
            while (newAction.ActionParameters == action.ActionParameters)
            {
                var actions = GetPossibleActions(newAction.EndPosition);
                if (random.Next(0, 1000) % 13 == 0)
                    break;

                AgentActionDTO selectedAction = null;
                foreach (var generatedFunction in actions)
                {
                    if (generatedFunction.ActionParameters == newAction.ActionParameters)
                    {
                        selectedAction = generatedFunction;
                        break;
                    }
                }
                
                if(selectedAction != null)
                {
                    newAction = selectedAction;
                    continue;
                }
                break;
            }

            action.EndPosition = newAction.EndPosition;
            return action;
        }

        public override void FinishAction() { }

        public override bool IsPerforming()
        {
            return false;
        }

        public override void ClearAction()
        {
            moveHelper = null;
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
