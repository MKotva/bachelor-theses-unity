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
        private bool onlyGrounded;
        private LinearTranslator moveHelper;

        public MoveAction(GameObject gameObject, float moveSpeed = 1, float moveSpeedCap = 1, bool canFallOf = false) : base(gameObject)
        {
            speed = moveSpeed;
            speedCap = moveSpeedCap;
            onlyGrounded = canFallOf;
        }

        /// <summary>
        /// Returns all possible AgentActions from given position. Possible actions is action, which leads
        /// to a walkable position.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
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
                reacheablePositions.Add(new AgentActionDTO(position, newPositions[0], "Move left", 1f, this));
            }

            if (IsWalkable(newPositions[1]))
            {
                reacheablePositions.Add(new AgentActionDTO(position, newPositions[1], "Move right", 1f, this));
            }

            return reacheablePositions;
        }

        /// <summary>
        /// Performs agent action by simulating movement of action performer.
        /// </summary>
        /// <param name="action">Agent action</param>
        /// <param name="actions">All queued agent action. Usefull for optimalization of actual action.</param>
        /// <param name="deltaTime">Time since last update.</param>
        /// <returns></returns>
        public override bool PerformAgentAction(AgentActionDTO action, Queue<AgentActionDTO> actions, float deltaTime)
        {
            if (moveHelper == null)
            {
                moveHelper = new LinearTranslator(speed, action.StartPosition, map.GetCellCenterPosition(LinearTranslator.FindContinuousPath(action, actions)));
            }

            if (!moveHelper.TranslationTick(performer, map, deltaTime))
            {
                return false;
            }

            moveHelper = null;
            return true;
        }

        /// <summary>
        /// Simulates action, based on given AgentAction, by printing results of simulated action.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public override List<GameObject> PrintAgentAction(AgentActionDTO action)
        {
            return new List<GameObject>() { map.Marker.CreateMarkAtPosition(action.StartPosition) };
        }

        /// <summary>
        /// Performs action in direction, based on given string parameter.
        /// </summary>
        /// <param name="action">Action parameter</param>
        public override void PerformAction(string action)
        {
            if (actionTypes.ContainsKey(action))
            {
                if (!MotionHelper.CheckIfStaysOnGround(performer) && onlyGrounded)
                {
                    return;
                }

                var direction = actionTypes[action];
                performerRigidbody.AddForce(direction * speed);
                ClampSpeed();
            }
        }

        /// <summary>
        /// Returns random action from all possible actions on given position.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Clears actualy performed agent action.
        /// </summary>
        public override void ClearAction()
        {
            moveHelper = null;
        }

        /// <summary>
        /// Checks if actual action contains given action type code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public override bool ContainsActionCode(string code)
        {
            return ActionTypes.Contains(code);
        }

        /// <summary>
        /// Clamps speed for this action.
        /// </summary>
        public override void ClampSpeed()
        {
            if (speedCap == 0)
                return;

            var velocity = performerRigidbody.velocity;
            if (velocity.x > speedCap ||
                velocity.x < -speedCap)
                performerRigidbody.velocity = Vector2.ClampMagnitude(performerRigidbody.velocity, speedCap);
        }
    }
}
