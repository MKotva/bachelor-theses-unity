using Assets.Core.GameEditor.AIActions.Movement;
using Assets.Core.GameEditor.DTOS;
using Assets.Scenes.GameEditor.Core.AIActions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.GameEditor.AI
{
    public class MoveAction : ActionBase
    {
        private static Dictionary<string, Vector2> actionTypes = new Dictionary<string, Vector2>
        {
            { "Move left", Vector2.left },
            { "Move right", Vector2.right },
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
        private bool GroundedOnly;
        private LinearTranslator moveHelper;

        public MoveAction(GameObject gameObject, float moveSpeed = 1, float moveSpeedCap = 1, bool canFallOf = false) : base(gameObject)
        {
            speed = moveSpeed;
            speedCap = moveSpeedCap;
            GroundedOnly = canFallOf;
        }

        public override List<AgentActionDTO> GetPossibleActions(Vector2 position)
        {
            var cellSize = map.GridLayout.cellSize;
            var newPositions = new Vector2[]
            {
                map.GetCellCenterPosition(new Vector2(position.x - cellSize.x, position.y)), //Left
                map.GetCellCenterPosition(new Vector2(position.x + cellSize.x, position.y)), //Right
            };

            var reacheablePositions = new List<AgentActionDTO>();
            if (IsWalkable(newPositions[0]))
            {
                reacheablePositions.Add(new AgentActionDTO(position, newPositions[0], "Move left", 1f, PerformAgentActionAsync, PrintAgentActionAsync));
            }

            if (IsWalkable(newPositions[1]))
            {
                reacheablePositions.Add(new AgentActionDTO(position, newPositions[1], "Move right", 1f, PerformAgentActionAsync, PrintAgentActionAsync));
            }

            return reacheablePositions;
        }

        public override bool PerformAgentActionAsync(AgentActionDTO action, Queue<AgentActionDTO> actions, float deltaTime)
        {
            if (moveHelper == null)
            {
                moveHelper = new LinearTranslator(performerRigidbody, speed, action.StartPosition, map.GetCellCenterPosition(LinearTranslator.FindContinuousPath(action, actions)));
            }

            if (!moveHelper.TranslationTick(performer, map, deltaTime))
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
            if (actionTypes.ContainsKey(action))
            {
                if (MotionHelper.CheckIfStaysOnGround(performer))
                {
                    var direction = actionTypes[action];
                    performerRigidbody.AddForce(direction * speed);
                    performerRigidbody.velocity = Vector3.ClampMagnitude(performerRigidbody.velocity, speedCap);
                }
            }
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
                if (random.Next(0, 1000) % 9 == 0)
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

                if (selectedAction != null)
                {
                    newAction = selectedAction;
                    continue;
                }
                break;
            }

            action.EndPosition = newAction.EndPosition;
            return action;
        }

        public override bool IsPerforming()
        {
            return false;
        }

        public override void ClearAction()
        {
            moveHelper = null;
        }

        public override void FinishAction() { }

        public override bool ContainsActionCode(string code)
        {
            return ActionTypes.Contains(code);
        }
    }
}
