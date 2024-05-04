using Assets.Core.GameEditor.AIActions.Movement;
using Assets.Core.GameEditor.DTOS;
using Assets.Core.GameEditor.DTOS.Action;
using Assets.Scenes.GameEditor.Core.AIActions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public static List<string> ActionTypes
        {
            get
            {
                return actionTypes.Keys.ToList();
            }
        }

        private float minVertical;
        private float maxVertical;
        private float minHorizontal;
        private float maxHorizontal;
        
        private float chargeTimeMax;
        private float chargeTimeStart;
        private Vector2 chargeJumpDirection;

        private bool isPerforming;
        private bool OnlyGrounded;

        private JumperDTO jumperDTO;


        public ChargeableJumpAction(GameObject jumpingObject, float verticalMin = 1, float verticalMax = 1, float horizontalMin = 1, float horizontalMax = 1, float maxChargeTime = 1, bool onlyGrounded = false) : base(jumpingObject, 50)
        {
            performer = jumpingObject;
            minVertical = verticalMin;
            maxVertical = verticalMax;
            minHorizontal = horizontalMin;
            maxHorizontal = horizontalMax;
            chargeTimeMax = maxChargeTime;
            OnlyGrounded = onlyGrounded;

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

        public override List<AgentActionDTO> GetPossibleActions(Vector2 position)
        {
            var reacheablePositions = new List<AgentActionDTO>();
            var trajectories = GetAllPossibleTrajestories(position);

            foreach (var item in trajectories)
            {
                var centertedEndPoition = map.GetCellCenterPosition(item.EndPosition);
                if (IsWalkable(centertedEndPoition))
                {
                    var action = new AgentActionDTO(position, centertedEndPoition, $"{item.MotionDirection.x}:{item.MotionDirection.y}", 50, PerformAgentActionAsync, PrintAgentActionAsync);
                    reacheablePositions.Add(action);
                }
            }

            return reacheablePositions;
        }

        public override bool PerformAgentActionAsync(AgentActionDTO action, Queue<AgentActionDTO> actions, float deltaTime)
        {
            var jumpDirection = MathHelper.GetVector3FromString(action.ActionParameters);
            performerRigidbody.AddForce(jumpDirection * 50);

            while (IsPerforming())
            {
                return false;
            }

            performer.transform.position = map.GetCellCenterPosition(action.EndPosition);
            return true;
        }

        public override AgentActionDTO GetRandomAction(Vector2 lastPosition)
        {
            var actions = GetPossibleActions(lastPosition);
            if(actions.Count == 0)
                return null;

            return actions[(int) random.Next(0, actions.Count)];
        }

        public override async Task<List<GameObject>> PrintAgentActionAsync(AgentActionDTO action)
        {
            var trajectory = TrajectoryCalculator.GetTrajectory(jumperDTO ,action.StartPosition, MathHelper.GetVector3FromString(action.ActionParameters));
            var result = map.Marker.CreateMarkAtPosition(map.Marker.MarkerDotPrefab, trajectory.Path);
            return await Task.FromResult(result);
        }

        public override bool IsPerforming()
        {
            RaycastHit2D hit = Physics2D.Raycast(performer.transform.position, Vector2.down, map.GridLayout.cellSize.y * 0.6f, LayerMask.GetMask("Box"));
            if (hit.collider != null)
                return false;
            return true;
        }

        public override List<GameObject> PrintReacheables(Vector2 startPosition)
        {
            return map.Marker.CreateMarkAtPosition(map.Marker.MarkerDotPrefab, GetReacheablePositions(startPosition));
        }

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

        public override void PerformAction(string action)
        {
            if (!actionTypes.ContainsKey(action) || isPerforming)
                return;

            if (MoveHelper.CheckIfStaysOnGround(performer))
            {
                chargeTimeStart = Time.time;
                chargeJumpDirection = actionTypes[action];
                isPerforming = true;
            }
        }

        public override void FinishAction()
        {
            var spentTime = Time.time - chargeTimeStart < chargeTimeMax ? Time.time - chargeTimeStart : chargeTimeMax;
            var percent = (spentTime / (chargeTimeMax / 100f));
            var additionVertical = ( maxVertical - minVertical ) * ( percent / 100 );
            var additionHorizontal = ( maxHorizontal - minHorizontal ) * ( percent / 100 );

            var jumpVector = TrajectoryCalculator.GetJumpVector(chargeJumpDirection, minVertical + additionVertical, minHorizontal + additionHorizontal);
            performerRigidbody.AddForce(jumpVector);

            isPerforming = false;
        }

        public override bool ContainsActionCode(string code)
        {
            return ActionTypes.Contains(code);
        }
        #region PRIVATE

        private List<TrajectoryDTO> GetAllPossibleTrajestories(Vector2 startPosition)
        {
            var trajectories = new List<TrajectoryDTO>();
            foreach (var direction in actionTypes.Values)
            {
                GetTrajectories(startPosition, direction).ForEach(x => trajectories.Add(x));
            }

            return trajectories;
        }

        private List<TrajectoryDTO> GetTrajectories(Vector2 startPosition, Vector2 jumpDirection)
        {
            var trajectories = new List<TrajectoryDTO>();

            var power = new Vector2(maxHorizontal, maxVertical);
            var adjustment = new Vector2(maxHorizontal * 0.05f, maxVertical * 0.05f);

            while (power.x > minHorizontal && power.y > minVertical)
            {
                var trajectory = TrajectoryCalculator.GetTrajectory(jumperDTO, startPosition, TrajectoryCalculator.GetJumpVector(jumpDirection, power.y, power.x));
                
                if(trajectory != null)
                    trajectories.Add(trajectory);
                
                power = power - adjustment;
            }

            return trajectories;
        }
    }
    #endregion
}

