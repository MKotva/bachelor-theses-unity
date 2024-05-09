using Assets.Core.GameEditor;
using Assets.Core.GameEditor.AIActions;
using Assets.Core.GameEditor.AIActions.Movement;
using Assets.Core.GameEditor.DTOS;
using Assets.Core.GameEditor.DTOS.Action;
using Assets.Scenes.GameEditor.Core.AIActions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.GameEditor.AI
{
    public class JumpAction : ActionBase
    {

        private static Dictionary<string, Vector2> actionTypes = new Dictionary<string, Vector2>
        {
            { "Jump left", new Vector2(-1, 1) },
            { "Jump right", new Vector2(1, 1) },
            { "Jump up", Vector2.up }
        };
        public static List<string> ActionTypes
        {
            get
            {
                return actionTypes.Keys.ToList();
            }
        }

        private JumperDTO jumperDTO;
        private SimpleJumpActionDTO jumpSetting;
        private bool isPerforming;
        private bool startedAgentAction;
        public JumpAction(GameObject jumpingObject, SimpleJumpActionDTO jumpSetting) : base(jumpingObject, 50)
        {
            performer = jumpingObject;
            this.jumpSetting = jumpSetting;

            var collider = performer.GetComponent<Collider2D>();
            if (!collider.enabled)
                collider.enabled = true;

            var boxColliderSize = new Vector2(collider.bounds.extents.x, collider.bounds.extents.y);
            collider.enabled = false;

            var timeTick = ( Time.fixedDeltaTime / Physics2D.velocityIterations ) * 10;
            jumperDTO = new JumperDTO()
            {
                Performer = performer,
                ColliderSize = boxColliderSize,
                GravityAcceleration = Physics2D.gravity * performerRigidbody.gravityScale * MathHelper.Pow(timeTick, 2),
                Drag = 1f - timeTick * performerRigidbody.drag,
                Mass = performerRigidbody.mass,
                TimeTick = timeTick
            };
        }

        /// <summary>
        /// Returns all possible AgentActions from given position. Possible actions is action, which leads
        /// to a walkable position.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public override List<AgentActionDTO> GetPossibleActions(Vector2 position)
        {
            var reacheablePositions = new List<AgentActionDTO>();

            foreach (TrajectoryDTO trajectory in GetTrajectories(position))
            {
                var centertedEndPoition = map.GetCellCenterPosition(trajectory.EndPosition);
                if (IsWalkable(centertedEndPoition))
                {
                    var action = new AgentActionDTO(position, centertedEndPoition, $"{trajectory.MotionDirection.x}:{trajectory.MotionDirection.y}", 50, this);
                    reacheablePositions.Add(action);
                }
            }
            return reacheablePositions;
        }

        /// <summary>
        /// Performs agent action by simulating movement of action performer.
        /// </summary>
        /// <param name="action">Agent action</param>
        /// <param name="actions">All queued agent action. Usefull for optimalization of actual action.</param>
        /// <param name="deltaTime">Time since last update.</param>
        public override bool PerformAgentAction(AgentActionDTO action, Queue<AgentActionDTO> actions, float f)
        {
            var jumpDirection = MathHelper.GetVector3FromString(action.ActionParameters);
            if (!startedAgentAction)
            {
                if (performerRigidbody.isKinematic || performerRigidbody.IsSleeping())
                {
                    ActivateObject();
                    performerRigidbody.AddForce(jumpDirection);
                }

                if (MotionHelper.CheckIfStaysOnGround(performer))
                    return false;

                colliderController.ObjectCollider.isTrigger = true;
                startedAgentAction = true;
            }

            if (!MotionHelper.CheckIfStaysOnGround(performer))
            {
                return false;
            }

            colliderController.ObjectCollider.isTrigger = false;
            performer.transform.position = map.GetCellCenterPosition(action.EndPosition);
            startedAgentAction = false;
            DeactivateObject();
            return true;
        }

        /// <summary>
        /// Simulates action, based on given AgentAction, by printing results of simulated action.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public override List<GameObject> PrintAgentAction(AgentActionDTO action)
        {
            var trajectory = TrajectoryCalculator.GetTrajectory(jumperDTO, action.StartPosition, MathHelper.GetVector3FromString(action.ActionParameters));
            return map.Marker.CreateMarkAtPosition(map.Marker.MarkerDotPrefab, trajectory.Path);
        }


        /// <summary>
        /// Performs action in direction, based on given string parameter.
        /// </summary>
        /// <param name="action">Action parameter</param>
        public override void PerformAction(string action)
        {
            if (!actionTypes.ContainsKey(action) || isPerforming)
                return;

            if (!MotionHelper.CheckIfStaysOnGround(performer) && jumpSetting.OnlyGrounded)
            {
                return;
            }

            performerRigidbody.AddForce(TrajectoryCalculator.GetJumpVector(actionTypes[action], jumpSetting.VerticalForce, jumpSetting.HorizontalForce));
            isPerforming = true;
        }

        /// <summary>
        /// This method finishes actual movement action -> after event is no longer trigerred.
        /// </summary>
        public override void FinishAction() 
        {
            isPerforming = false;
        }

        /// <summary>
        /// Returns random action from all possible actions on given position.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public override AgentActionDTO GetRandomAction(Vector2 lastPosition)
        {
            var actions = GetPossibleActions(lastPosition);
            if (actions.Count == 0)
                return null;

            return actions[(int) random.Next(0, actions.Count)];
        }

        /// <summary>
        /// Prints all reacheable positions with this aciton. Founded position has to be
        /// "Walkable". That means it has to contain collider beneath it.
        /// </summary>
        /// <param name="startPosition">Start position</param>
        /// <returns></returns>
        public override List<GameObject> PrintReacheables(Vector2 startPosition)
        {
            var positions = new List<Vector2>();

            var actions = GetPossibleActions(startPosition);
            foreach (var action in actions)
                positions.Add(action.EndPosition);

            return map.Marker.CreateMarkAtPosition(map.Marker.MarkerDotPrefab, positions);
        }

        /// <summary>
        /// Prints trajectories of all posible jumps from given position.
        /// </summary>
        /// <param name="position">Start position</param>
        /// <returns></returns>
        public List<GameObject> PrintAllPossibleJumps(Vector2 position)
        {
            var markers = new List<GameObject>();
            foreach (TrajectoryDTO trajectory in GetTrajectories(position))
            {
                map.Marker.CreateMarkAtPosition(map.Marker.MarkerDotPrefab, trajectory.Path).ForEach(x => markers.Add(x));
            }

            return markers;
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
        /// Clears actualy performed agent action.
        /// </summary>
        public override void ClearAction()
        {
            startedAgentAction = false;
            DeactivateObject();
        }

        /// <summary>
        /// Clamps speed for this action.
        /// </summary>
        public override void ClampSpeed()
        {
            if (jumpSetting.SpeedCap == 0)
                return;

            var velocity = performerRigidbody.velocity;
            if (velocity.y > jumpSetting.SpeedCap ||
                velocity.x > jumpSetting.SpeedCap ||
                velocity.x < -jumpSetting.SpeedCap)
                performerRigidbody.velocity = Vector2.ClampMagnitude(performerRigidbody.velocity, jumpSetting.SpeedCap);
        }

        private List<TrajectoryDTO> GetTrajectories(Vector2 position)
        {
            return new List<TrajectoryDTO>
            {
                TrajectoryCalculator.GetTrajectory(jumperDTO, position, TrajectoryCalculator.GetJumpVector(Vector2.left, jumpSetting.VerticalForce, jumpSetting.HorizontalForce)),
                TrajectoryCalculator.GetTrajectory(jumperDTO, position, TrajectoryCalculator.GetJumpVector(Vector2.right, jumpSetting.VerticalForce, jumpSetting.HorizontalForce))
            };
        }
    }
}
