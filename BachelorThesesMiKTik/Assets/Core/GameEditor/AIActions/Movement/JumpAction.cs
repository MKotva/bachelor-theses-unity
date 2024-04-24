using Assets.Core.GameEditor;
using Assets.Core.GameEditor.AIActions;
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
            { "Jump left", Vector2.left },
            { "Jump right", Vector2.right },
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

        public JumpAction(GameObject jumpingObject, float jumpForce = 5, float moveSpeed = 2) : base(jumpingObject, 50)
        {
            performer = jumpingObject;
            forceUp = jumpForce;
            forceInDirection = moveSpeed;

            var boxCollider = performer.GetComponent<BoxCollider2D>();

            if (!boxCollider.enabled)
                boxCollider.enabled = true;

            var boxColliderSize = new Vector2(boxCollider.bounds.extents.x, boxCollider.bounds.extents.y);
            boxCollider.enabled = false;

            var timeTick = ( Time.fixedDeltaTime / Physics2D.velocityIterations ) * 10;
            jumperDTO = new JumperDTO()
            {
                ColliderSize = boxColliderSize,
                GravityAcceleration = Physics2D.gravity * performerRigidbody.gravityScale * MathHelper.Pow(timeTick, 2),
                Drag = 1f - timeTick * performerRigidbody.drag
            };
        }

        public override List<AgentActionDTO> GetPossibleActions(Vector3 position)
        {
            var reacheablePositions = new List<AgentActionDTO>();

            foreach (TrajectoryDTO trajectory in GetTrajectories(position))
            {
                if (IsWalkable(trajectory.EndPosition))
                {
                    var action = new AgentActionDTO(position, trajectory.EndPosition, $"{trajectory.MotionDirection.x}:{trajectory.MotionDirection.y}", 50, PerformAgentActionAsync, PrintAgentActionAsync);
                    reacheablePositions.Add(action);
                }
            }
            return reacheablePositions;
        }

        public override async Task PerformAgentActionAsync(AgentActionDTO action)
        {
            var jumpDirection = MathHelper.GetVector3FromString(action.ActionParameters);
            performerRigidbody.AddForce(jumpDirection);

            await Task.Delay(100);
            while (IsPerforming())
            {
                await Task.Delay(100);
            }

            performer.transform.position = map.GetCellCenterPosition(action.EndPosition);
            await Task.Delay(1000);
        }

        public override async Task<List<GameObject>> PrintAgentActionAsync(AgentActionDTO action)
        {
            var trajectory = JumpHelper.GetTrajectory(jumperDTO, action.StartPosition, MathHelper.GetVector3FromString(action.ActionParameters));
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

        public override List<GameObject> PrintReacheables(Vector3 startPosition)
        {
            return map.Marker.CreateMarkAtPosition(map.Marker.MarkerDotPrefab, GetReacheablePositions(startPosition));
        }

        public List<GameObject> PrintAllPossibleJumps(Vector3 position)
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

            if(JumpHelper.CheckIfStaysOnGround(performer))
            {
                performerRigidbody.AddForce(JumpHelper.GetJumpVector(actionTypes[action], forceUp, forceInDirection));
                Vector3.ClampMagnitude(performerRigidbody.velocity, 50);
            }
        }

        private List<TrajectoryDTO> GetTrajectories(Vector2 position)
        {
            return new List<TrajectoryDTO>
            {
                JumpHelper.GetTrajectory(jumperDTO, position, JumpHelper.GetJumpVector(Vector2.left, forceUp, forceInDirection)),
                JumpHelper.GetTrajectory(jumperDTO, position, JumpHelper.GetJumpVector(Vector2.right, forceUp, forceInDirection))
            };
        }
    }
}
