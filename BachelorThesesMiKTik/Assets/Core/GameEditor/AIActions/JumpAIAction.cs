using Assets.Core.GameEditor;
using Assets.Core.GameEditor.DTOS;
using Assets.Scenes.GameEditor.Core.AIActions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.ExceptionServices;
using UnityEngine;
using UnityEngine.UIElements;
using static DG.Tweening.DOTweenModuleUtils;

namespace Assets.Scripts.GameEditor.AI
{
    public class JumpAIAction : AIActionBase
    {
        private GameObject _jumpingObject;
        private Rigidbody2D _rigid;
        private List<GameObject> _markers;
        private float _defaultBounceForce;
        private float _jumpForce;
        private float _moveSpeed;
        private bool _shouldPrint;

        private Vector2 gravityAcceleration;

        private float timeTick;
        private float drag;

        private int depth;

        public JumpAIAction(MapCanvasController context, GameObject jumpingObject, float jumpForce = 5, float moveSpeed = 2) : base(context, 50)
        {
            _jumpingObject = jumpingObject;
            _rigid = _jumpingObject.GetComponent<Rigidbody2D>();

            _markers = new List<GameObject>();
            _defaultBounceForce = 0.1f;
            _shouldPrint = false;

            _jumpForce = jumpForce;
            _moveSpeed = moveSpeed;

            timeTick = ( Time.fixedDeltaTime / Physics2D.velocityIterations ) * 10;
            gravityAcceleration = Physics2D.gravity * _rigid.gravityScale * MathHelper.Pow(timeTick, 2);
            drag = 1f - timeTick * _rigid.drag;

            depth = 30;
        }

        public JumpAIAction(MapCanvasController context, GameObject jumpingObject, float defaultBounceForce, float jumpForce, float moveSpeed) : this(context, jumpingObject, jumpForce, moveSpeed)
        {
            _defaultBounceForce = defaultBounceForce;
        }

        public override List<AgentActionDTO> GetReacheablePosition(Vector3 position)
        {
            var reacheablePositions = new List<AgentActionDTO>();

            var right = GetAllJumpTrajectories(position, Vector2.right, _jumpForce, _moveSpeed);
            var left = GetAllJumpTrajectories(position, Vector2.left, _jumpForce, _moveSpeed);

            
            foreach ( var item in right) 
            {
                if(IsWalkable(item.Position))
                {
                    var action = new AgentActionDTO(position, item.Position, $"{item.MotionPower.x}:{item.MotionPower.y}", 50, PerformAction, PerformActionWithPrint);
                    reacheablePositions.Add(action);
                }
            }

            foreach (var item in left)
            {
                if (IsWalkable(item.Position))
                {
                    var action = new AgentActionDTO(position, item.Position, $"{item.MotionPower.x}:{item.MotionPower.y}", 50, PerformAction, PerformActionWithPrint);
                    reacheablePositions.Add(action);
                }
            }

            return reacheablePositions;
        }

        public override void PerformAction(Vector3 startPosition, string parameters)
        {
            //_shouldPrint = true;
            //context.CreateMarkAtPosition(startPosition);
            //GetTrajectory(startPosition, MathHelper.GetVector3FromString(parameters));
            //_shouldPrint = false;
        }

        public override List<GameObject> PerformActionWithPrint(Vector3 startPosition, string parameters)
        {
            _shouldPrint = true;
            _markers.Add(context.CreateMarkAtPosition(startPosition));
            GetTrajectory(startPosition, MathHelper.GetVector3FromString(parameters));
            _shouldPrint = false;
            return _markers;
        }

        public List<GameObject> PrintJumpables()
        {
            ClearMarks();
            _shouldPrint = true;
            GetReacheablePosition(_jumpingObject.transform.position);
            _shouldPrint = false;
            return _markers;
        }

        private void ClearMarks()
        {
            for (int i = 0; i < _markers.Count; i++)
            {
                context.DestroyMark(_markers[i]);
            }
            _markers.Clear();
        }

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
                jumpRecords.Add(new JumpPositionDTO(result.Last(), motionDirection));
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

                var centered = context.GetCellCenterPosition(position);
                if (context.ContainsObjectAtPosition(centered, new int[] { 7, 8 })) //TODO: For every ai object, have list of layers.
                {
                    var hasLanded = false;
                    actualDirection = HandleCollision(context.GetObjectAtPosition(centered), previousPos, previousDir, out position, out hasLanded);
                    if (hasLanded)
                        return trajectoryPoints;
                }

                if (!trajectoryPoints.Contains(centered))
                    trajectoryPoints.Add(centered);

                if (_shouldPrint)
                    PrintMarkers(position);

                previousDir = actualDirection;
                previousPos = position;
            }
            return trajectoryPoints;
        }

        private Vector2 Bounce(Vector2 startPosition, Vector2 motionDirection, out Vector2 hitPosition, out bool isLandPossible)
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

                if(hit.normal == Vector2.zero)
                {
                    isLandPossible = true;
                    Debug.DrawRay(startPosition, motionDirection, Color.red, 100000000000000000);
                }

                hitPosition = hit.point;
                return Vector2.Reflect(motionDirection, normal);
            }

            hitPosition = Vector2.zero;
            return Vector2.zero;
        }

        private Vector2 HandleCollision(GameObject hittedObject, Vector2 startPos, Vector2 motionDirection, out Vector2 hitPosition, out bool isLandingPossible)
        {
            var bounceDirection = Bounce(startPos, motionDirection, out hitPosition, out isLandingPossible);

            BoxCollider2D collider2D;
            if (hittedObject.TryGetComponent(out collider2D))
            {
                if (!isLandingPossible || collider2D.sharedMaterial.bounciness > 0.1)
                {
                    bounceDirection *= collider2D.sharedMaterial.bounciness;
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

        private void PrintMarkers(Vector2 position)
        {
            var mark = context.CreateMarkAtPosition(context.MarkerDotPrefab, position);
            _markers.Add(mark);
        }
    }
}
