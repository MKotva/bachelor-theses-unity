using Assets.Core.GameEditor;
using Assets.Core.GameEditor.DTOS;
using Assets.Scenes.GameEditor.Core.AIActions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.GameEditor.AI
{
    public class JumpAIAction : AIActionBase
    {

        private static List<string> actionTypes = new List<string>
        {
            "Jump left",
            "Jump right",
            "Jump up"
        };
        public static List<string> ActionTypes
        {
            get
            {
                return new List<string>(actionTypes);
            }
        }

        private GameObject performer;
        private Rigidbody2D _rigid;

        private float _defaultBounceForce = 0.1f;
        private float _jumpForce;
        private float _moveSpeed;

        private Vector2 _boxColliderSize;
        private Vector2 gravityAcceleration;

        private float timeTick;
        private float drag;

        private int depth;

        public JumpAIAction(GameObject jumpingObject, float jumpForce = 5, float moveSpeed = 2) : base(50)
        {
            performer = jumpingObject;
            _rigid = performer.GetComponent<Rigidbody2D>();
            var boxCollider = performer.GetComponent<BoxCollider2D>();

            if (!boxCollider.enabled)
                boxCollider.enabled = true;

            _boxColliderSize = new Vector2(boxCollider.bounds.extents.x, boxCollider.bounds.extents.y);
            boxCollider.enabled = false;

            _jumpForce = jumpForce;
            _moveSpeed = moveSpeed;

            timeTick = ( Time.fixedDeltaTime / Physics2D.velocityIterations ) * 10;
            gravityAcceleration = Physics2D.gravity * _rigid.gravityScale * MathHelper.Pow(timeTick, 2);
            drag = 1f - timeTick * _rigid.drag;

            depth = 40;
        }

        public JumpAIAction(GameObject jumpingObject, float defaultBounceForce, float jumpForce, float moveSpeed) : this(jumpingObject, jumpForce, moveSpeed)
        {
            _defaultBounceForce = defaultBounceForce;
        }

        public override List<AgentActionDTO> GetPossibleActions(Vector3 position)
        {
            var reacheablePositions = new List<AgentActionDTO>();

            var right = GetAllJumpTrajectories(position, Vector2.right, _jumpForce, _moveSpeed);
            var left = GetAllJumpTrajectories(position, Vector2.left, _jumpForce, _moveSpeed);


            foreach (var item in right)
            {
                if (IsWalkable(item.Position))
                {
                    var action = new AgentActionDTO(position, item.Position, $"{item.MotionDirection.x}:{item.MotionDirection.y}", 50, PerformAgentActionAsync, PrintAgentActionAsync);
                    reacheablePositions.Add(action);
                }
            }

            foreach (var item in left)
            {
                if (IsWalkable(item.Position))
                {
                    var action = new AgentActionDTO(position, item.Position, $"{item.MotionDirection.x}:{item.MotionDirection.y}", 50, PerformAgentActionAsync, PrintAgentActionAsync);
                    reacheablePositions.Add(action);
                }
            }

            return reacheablePositions;
        }

        public override async Task PerformAgentActionAsync(AgentActionDTO action)
        {
            var jumpDirection = MathHelper.GetVector3FromString(action.PositionActionParameter);
            _rigid.AddForce(jumpDirection * 50);

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
            var trajectory = GetTrajectory(action.StartPosition, MathHelper.GetVector3FromString(action.PositionActionParameter));
            var result = map.Marker.CreateMarkAtPosition(map.Marker.MarkerDotPrefab, trajectory);
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
            List<List<Vector3>> trajectories;
            GetAllJumpTrajectories(position, Vector2.right, _jumpForce, _moveSpeed, out trajectories);

            List<List<Vector3>> trajectoriesLeft;
            GetAllJumpTrajectories(position, Vector2.left, _jumpForce, _moveSpeed, out trajectoriesLeft);

            trajectoriesLeft.ForEach(x => trajectories.Add(x));

            var markers = new List<GameObject>();
            foreach (var trajectory in trajectories)
            {
                map.Marker.CreateMarkAtPosition(map.Marker.MarkerDotPrefab, trajectory).ForEach(x => markers.Add(x));
            }

            return markers;
        }

        #region PRIVATE

        private List<JumpPositionDTO> GetAllJumpTrajectories(Vector2 position, Vector2 jumpDirection, float jumpPower, float motionPower)
        {
            List<List<Vector3>> dummy;
            return GetAllJumpTrajectories(position, jumpDirection, jumpPower, motionPower, out dummy);
        }

        private List<JumpPositionDTO> GetAllJumpTrajectories(Vector2 startPosition, Vector2 jumpDirection, float jumpPower, float motionPower, out List<List<Vector3>> trajectories)
        {
            trajectories = new List<List<Vector3>>();
            var jumpRecords = new List<JumpPositionDTO>();
            var position = startPosition;

            var adjustedJumppower = jumpPower;
            float adjustment = jumpPower * 0.05f;

            while (adjustedJumppower > 1)
            {
                var motionDirection = ( Vector2.up * adjustedJumppower ) - ( jumpDirection * motionPower );
                var result = GetTrajectory(position, motionDirection);
                trajectories.Add(result);
                jumpRecords.Add(new JumpPositionDTO(map.GetCellCenterPosition(result.Last()), motionDirection));
                adjustedJumppower = adjustedJumppower - adjustment;
            }

            return jumpRecords;
        }

        private List<Vector3> GetTrajectory(Vector2 startPos, Vector2 initialDirection)
        {
            var trajectoryPoints = new List<Vector3>();
            var position = startPos;
            var actualDirection = initialDirection * timeTick;

            var previousPos = position;
            var previousDir = actualDirection;

            while (trajectoryPoints.Count < depth)
            {
                actualDirection += gravityAcceleration;
                actualDirection *= drag;
                position = MathHelper.Add(position, actualDirection);

                Vector2 previousHitPosition;
                GameObject hittedObject;
                if (CheckCollision(position, previousPos, out previousHitPosition, out hittedObject)) //TODO: For every ai object, have list of layers.
                {
                    bool hasLanded;
                    actualDirection = HandleCollision(hittedObject, previousHitPosition, previousDir, out hasLanded);
                    if (hasLanded)
                        return trajectoryPoints;
                }

                trajectoryPoints.Add(position);

                previousDir = actualDirection;
                previousPos = position;
            }
            return trajectoryPoints;
        }

        private bool CheckCollision(Vector2 position, Vector2 PreviousPostion, out Vector2 preHitPositions, out GameObject hittedObject)
        {
            var actualPositionCorners = GetCorners(position);
            var previousPositionCorners = GetCorners(PreviousPostion);

            for (int i = 0; i < actualPositionCorners.Length; i++)
            {
                var centered = map.GetCellCenterPosition(actualPositionCorners[i]);
                if (map.ContainsObjectAtPosition(centered, new int[] { 7, 8 })) //TODO: For every ai object, have list of layers.
                {
                    hittedObject = map.GetObjectAtPosition(centered);
                    preHitPositions = previousPositionCorners[i];
                    return true;
                }
            }
            hittedObject = null;
            preHitPositions = Vector2.zero;
            return false;
        }
        private Vector2 Bounce(Vector2 startPosition, Vector2 motionDirection, out bool isLandPossible)
        {
            isLandPossible = false;
            RaycastHit2D hit = Physics2D.Raycast(startPosition, motionDirection, 1, LayerMask.GetMask("Box"));
            if (hit.collider != null)
            {
                var normal = hit.normal;
                if (( hit.normal.x > 0 && hit.normal.x < 1 ) &&
                    ( hit.normal.y > 0 && hit.normal.y < 1 ))
                {
                    if (hit.normal.y > 0)
                        normal = Vector2.up;
                    else
                        normal = Vector2.down;
                }

                if (hit.normal == Vector2.up)
                    isLandPossible = true;

                if (hit.normal == Vector2.zero)
                {
                    isLandPossible = true;
                }

                return Vector2.Reflect(motionDirection, normal);
            }
            return Vector2.zero;
        }

        private Vector2 HandleCollision(GameObject hittedObject, Vector2 startPos, Vector2 motionDirection, out bool isLandingPossible)
        {
            var bounceDirection = Bounce(startPos, motionDirection, out isLandingPossible);

            BoxCollider2D collider2D;
            if (hittedObject.TryGetComponent(out collider2D))
            {
                if (!isLandingPossible)
                {
                    bounceDirection *= collider2D.sharedMaterial.bounciness + 1.1f;
                    isLandingPossible = false;
                }
            }
            else
            {
                bounceDirection *= _defaultBounceForce;
                isLandingPossible = false;
            }
            return bounceDirection;
        }

        private Vector2[] GetCorners(Vector2 position)
        {
            var maxPosition = position + _boxColliderSize;
            var minPosition = position - _boxColliderSize;
            return new Vector2[]
            {
                maxPosition,
                minPosition,
                new Vector2(maxPosition.x, minPosition.y),
                new Vector2(minPosition.y, maxPosition.x),
            };
        }

        public override void PerformAction(string action)
        {
            throw new System.NotImplementedException();
        }
    }
    #endregion
}
