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

        public JumpAIAction(MapCanvasController context, GameObject jumpingObject, float jumpForce = 5, float moveSpeed = 2) : base(context, 50)
        {
            _jumpingObject = jumpingObject;
            _rigid = _jumpingObject.GetComponent<Rigidbody2D>();

            _markers = new List<GameObject>();
            _defaultBounceForce = 0.1f;
            _shouldPrint = false;

            _jumpForce = jumpForce;
            _moveSpeed = moveSpeed;
        }

        public JumpAIAction(MapCanvasController context, GameObject jumpingObject, float defaultBounceForce, float jumpForce, float moveSpeed) : this(context, jumpingObject, jumpForce, moveSpeed)
        {
            _defaultBounceForce = defaultBounceForce;
        }

        public override AgentActionDTO GetReacheablePosition(Vector3 position)
        {
            var right = GetAllJumpTrajectories(position, Vector2.right, _jumpForce, _moveSpeed, 30);
            var left = GetAllJumpTrajectories(position, Vector2.left, _jumpForce, _moveSpeed, 30);


            var reacheable = new List<Vector3>();
            var parameters = new List<string>();
            foreach ( var item in right) 
            {
                if(IsWalkable(item.Position))
                {
                    reacheable.Add(item.Position);
                    parameters.Add(item.MotionPower.ToString());
                }
            }

            foreach (var item in left)
            {
                if (IsWalkable(item.Position))
                {
                    reacheable.Add(item.Position);
                    parameters.Add(item.MotionPower.ToString());
                }
            }

            return new AgentActionDTO(reacheable, parameters, PerformAction, actionCost);
        }

        public override void PerformAction(string parameters)
        {
            throw new NotImplementedException();
        }

        public void PrintJumpables()
        {
            ClearMarks();
            _shouldPrint = true;
            GetReacheablePosition(_jumpingObject.transform.position);
            _shouldPrint = false;
        }

        private void ClearMarks()
        {
            for (int i = 0; i < _markers.Count; i++)
            {
                context.DestroyMark(_markers[i]);
            }
            _markers.Clear();
        }

        private List<JumpPositionDTO> GetAllJumpTrajectories(Vector2 position, Vector2 jumpDirection, float jumpPower, float motionPower, int depth)
        {
            List<List<Vector3>> dummy;
            return GetAllJumpTrajectories(position, jumpDirection, jumpPower, motionPower, depth, out dummy);
        }

        private List<JumpPositionDTO> GetAllJumpTrajectories(Vector2 startPosition, Vector2 jumpDirection, float jumpPower, float motionPower, int depth, out List<List<Vector3>> trajectories)
        {
            trajectories = new List<List<Vector3>>();
            var jumpRecords = new List<JumpPositionDTO>();
            var position = startPosition;

            var timeTick = ( Time.fixedDeltaTime / Physics2D.velocityIterations ) * 10;
            var gravityAcceleration = Physics2D.gravity * _rigid.gravityScale * MathHelper.Pow(timeTick, 2);
            var drag = 1f - timeTick * _rigid.drag;

            var adjustedJumppower = jumpPower;
            float adjustment = jumpPower * 0.05f;

            while (adjustedJumppower > 1)
            {
                var motionDirection = ( Vector2.up * adjustedJumppower ) - ( jumpDirection * motionPower );
                var result = GetTrajectory(position, motionDirection, gravityAcceleration, timeTick, drag, depth);
                jumpRecords.Add(new JumpPositionDTO(result.Last(), motionDirection));
                adjustedJumppower = adjustedJumppower - adjustment;
            }

            return jumpRecords;
        }

        private List<Vector3> GetTrajectory(Vector2 startPos, Vector2 initialDirection, Vector2 gravityAcceleration, float timeTick, float drag, int depth)
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
            RaycastHit2D hit = Physics2D.Raycast(startPosition, motionDirection, Mathf.Infinity, LayerMask.GetMask("Box"));
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
