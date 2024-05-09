using Assets.Core.GameEditor.AIActions.Movement;
using Assets.Core.GameEditor.DTOS;
using Assets.Core.GameEditor.DTOS.Action;
using Assets.Scenes.GameEditor.Core.AIActions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Core.GameEditor.AIActions
{
    public class ChargeableJumpAction : ActionBase
    {
        private static Dictionary<string, Vector2> actionTypes = new Dictionary<string, Vector2>
        {
            { "Jump left", new Vector2(-1, 1) },
            { "Jump right", new Vector2(1, 1) },
            { "Jump up", Vector2.up }
        };

        /// <summary>
        /// All avalible action with string code.
        /// </summary>
        public static List<string> ActionTypes
        {
            get
            {
                return actionTypes.Keys.ToList();
            }
        }

        private ChargeJumpDTO jumpSetting;
        private JumperDTO jumperDTO;

        private float chargeTimeStart;
        private Vector2 chargeJumpDirection;
        private bool isPerforming;
        private bool startedAgentAction;

        public ChargeableJumpAction(GameObject jumpingObject, ChargeJumpDTO chargeJumpDTO) : base(jumpingObject, 50)
        {
            performer = jumpingObject;
            jumpSetting = chargeJumpDTO;

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
                GravityAcceleration = Physics2D.gravity * performerRigidbody.gravityScale * ( MathHelper.Pow(timeTick, 2) ),
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
            var trajectories = GetAllPossibleTrajestories(position);

            foreach (var item in trajectories)
            {
                var centertedEndPoition = map.GetCellCenterPosition(item.EndPosition);
                if (IsWalkable(centertedEndPoition))
                {
                    var action = new AgentActionDTO(position, centertedEndPoition, $"{item.MotionDirection.x}:{item.MotionDirection.y}", 50, this);
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
        /// <returns></returns>
        public override bool PerformAgentAction(AgentActionDTO action, Queue<AgentActionDTO> actions, float deltaTime)
        {
            var jumpDirection = MathHelper.GetVector3FromString(action.ActionParameters);

            if(!startedAgentAction) 
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

            if(!MotionHelper.CheckIfStaysOnGround(performer))
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

            chargeTimeStart = Time.time;
            chargeJumpDirection = actionTypes[action];
            isPerforming = true;

        }

        /// <summary>
        /// This method finishes actual movement action -> after event is no longer trigerred.
        /// </summary>
        public override void FinishAction()
        {
            var spentTime = Time.time - chargeTimeStart < jumpSetting.JumpTimeCap ? Time.time - chargeTimeStart : jumpSetting.JumpTimeCap;
            var percent = ( spentTime / ( jumpSetting.JumpTimeCap / 100f ) );
            var additionVertical = ( jumpSetting.VerticalForceMax - jumpSetting.VerticalForceMin ) * ( percent / 100 );
            var additionHorizontal = ( jumpSetting.HorizontalForceMax - jumpSetting.HorizontalForceMin ) * ( percent / 100 );

            var jumpVector = TrajectoryCalculator.GetJumpVector(chargeJumpDirection, jumpSetting.HorizontalForceMin + additionVertical, jumpSetting.HorizontalForceMin + additionHorizontal);
            performerRigidbody.AddForce(jumpVector);

            ClampSpeed();
            isPerforming = false;
        }

        /// <summary>
        /// Prints trajectories of all posible jumps from given position.
        /// </summary>
        /// <param name="position">Start position</param>
        /// <returns></returns>
        public List<GameObject> PrintAllPossibleJumps(Vector2 position)
        {
            var trajectories = GetAllPossibleTrajestories(position);
            var markers = new List<GameObject>();
            foreach (var trajectory in trajectories)
            {
                map.Marker.CreateMarkAtPosition(map.Marker.MarkerDotPrefab, trajectory.Path).ForEach(x => markers.Add(x));
            }

            return markers;
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
        /// Checks if actual action contains given action type code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public override bool ContainsActionCode(string code)
        {
            return ActionTypes.Contains(code);
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
        #region PRIVATE

        /// <summary>
        /// Returns all posible jump trajectories from given position to all directions.
        /// </summary>
        /// <param name="startPosition"></param>
        /// <returns></returns>
        private List<TrajectoryDTO> GetAllPossibleTrajestories(Vector2 startPosition)
        {
            var trajectories = new List<TrajectoryDTO>();
            foreach (var direction in actionTypes.Values)
            {
                GetTrajectories(startPosition, direction).ForEach(x => trajectories.Add(x));
            }

            return trajectories;
        }

        /// <summary>
        /// Returns all posible jump trajectories from given position with initial direction (left, right).
        /// </summary>
        /// <param name="startPosition"></param>
        /// <param name="jumpDirection"></param>
        /// <returns></returns>
        private List<TrajectoryDTO> GetTrajectories(Vector2 startPosition, Vector2 jumpDirection)
        {
            var trajectories = new List<TrajectoryDTO>();

            var power = new Vector2(jumpSetting.HorizontalForceMax, jumpSetting.VerticalForceMax);
            var adjustment = new Vector2(jumpSetting.HorizontalForceMax * 0.05f, jumpSetting.VerticalForceMax * 0.05f);

            while (power.x > jumpSetting.HorizontalForceMin && power.y > jumpSetting.VerticalForceMin)
            {
                var trajectory = TrajectoryCalculator.GetTrajectory(jumperDTO, startPosition, TrajectoryCalculator.GetJumpVector(jumpDirection, power.y, power.x));

                if (trajectory != null)
                    trajectories.Add(trajectory);

                power = power - adjustment;
            }

            return trajectories;
        }

        /// <summary>
        /// Clamps speed for this action.
        /// </summary>
        public override void ClampSpeed()
        {
            if (jumpSetting.JumpSpeedCap == 0)
                return;

            var velocity = performerRigidbody.velocity;
            if (velocity.y > jumpSetting.JumpSpeedCap ||
                velocity.x > jumpSetting.JumpSpeedCap ||
                velocity.x < -jumpSetting.JumpSpeedCap)
                performerRigidbody.velocity = Vector2.ClampMagnitude(performerRigidbody.velocity, jumpSetting.JumpSpeedCap);
        }
    }
    #endregion
}

