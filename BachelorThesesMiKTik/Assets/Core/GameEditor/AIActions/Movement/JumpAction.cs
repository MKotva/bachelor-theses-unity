using Assets.Core.GameEditor;
using Assets.Core.GameEditor.AIActions;
using Assets.Core.GameEditor.AIActions.Movement;
using Assets.Core.GameEditor.DTOS;
using Assets.Core.GameEditor.DTOS.Action;
using Assets.Scenes.GameEditor.Core.AIActions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        private float forceUp;
        private float forceInDirection;
        private bool onlyGrounded;

        public JumpAction(GameObject jumpingObject, float jumpForce = 5, float moveSpeed = 2, bool onlyGrounded = false) : base(jumpingObject, 50)
        {
            performer = jumpingObject;
            forceUp = jumpForce;
            forceInDirection = moveSpeed;
            this.onlyGrounded = onlyGrounded;

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

        public override List<AgentActionDTO> GetPossibleActions(Vector2 position)
        {
            var reacheablePositions = new List<AgentActionDTO>();

            foreach (TrajectoryDTO trajectory in GetTrajectories(position))
            {
                var centertedEndPoition = map.GetCellCenterPosition(trajectory.EndPosition);
                if (IsWalkable(centertedEndPoition))
                {
                    var action = new AgentActionDTO(position, centertedEndPoition, $"{trajectory.MotionDirection.x}:{trajectory.MotionDirection.y}", 50, PerformAgentActionAsync, PrintAgentActionAsync);
                    reacheablePositions.Add(action);
                }
            }
            return reacheablePositions;
        }

        public override bool PerformAgentActionAsync(AgentActionDTO action, Queue<AgentActionDTO> actions, float f)
        {
            var jumpDirection = MathHelper.GetVector3FromString(action.ActionParameters);
            performerRigidbody.AddForce(jumpDirection);

            if(IsPerforming())
            {
                return false;
            }

            performer.transform.position = map.GetCellCenterPosition(action.EndPosition);
            return true;
        }

        public override async Task<List<GameObject>> PrintAgentActionAsync(AgentActionDTO action)
        {
            var trajectory = TrajectoryCalculator.GetTrajectory(jumperDTO, action.StartPosition, MathHelper.GetVector3FromString(action.ActionParameters));
            var result = map.Marker.CreateMarkAtPosition(map.Marker.MarkerDotPrefab, trajectory.Path);
            return await Task.FromResult(result);
        }

        public override AgentActionDTO GetRandomAction(Vector2 lastPosition)
        {
            var actions = GetPossibleActions(lastPosition);
            if (actions.Count == 0)
                return null;

            return actions[(int) random.Next(0, actions.Count)];
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
            var markers = new List<GameObject>();
            foreach (TrajectoryDTO trajectory in GetTrajectories(position))
            {
                map.Marker.CreateMarkAtPosition(map.Marker.MarkerDotPrefab, trajectory.Path).ForEach(x => markers.Add(x));
            }

            return markers;
        }

        public override void PerformAction(string action)
        {
            if (!actionTypes.ContainsKey(action)) 
                return;

            if(MotionHelper.CheckIfStaysOnGround(performer))
            {
                performerRigidbody.AddForce(TrajectoryCalculator.GetJumpVector(actionTypes[action], forceUp, forceInDirection));
                Vector2.ClampMagnitude(performerRigidbody.velocity, 50);
            }
        }

        public override void FinishAction() { }

        public override bool ContainsActionCode(string code)
        {
            return ActionTypes.Contains(code);
        }

        private List<TrajectoryDTO> GetTrajectories(Vector2 position)
        {
            return new List<TrajectoryDTO>
            {
                TrajectoryCalculator.GetTrajectory(jumperDTO, position, TrajectoryCalculator.GetJumpVector(Vector2.left, forceUp, forceInDirection)),
                TrajectoryCalculator.GetTrajectory(jumperDTO, position, TrajectoryCalculator.GetJumpVector(Vector2.right, forceUp, forceInDirection))
            };
        }
    }
}
