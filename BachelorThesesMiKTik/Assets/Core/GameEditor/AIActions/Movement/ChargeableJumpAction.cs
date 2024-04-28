﻿using Assets.Core.GameEditor.DTOS;
using Assets.Core.GameEditor.DTOS.Action;
using Assets.Scenes.GameEditor.Core.AIActions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

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

        private JumperDTO jumperDTO;


        public ChargeableJumpAction(GameObject jumpingObject, float verticalMin = 1, float verticalMax = 1, float horizontalMin = 1, float horizontalMax = 1, float maxChargeTime = 1) : base(jumpingObject, 50)
        {
            performer = jumpingObject;
            minVertical = verticalMin;
            maxVertical = verticalMax;
            minHorizontal = horizontalMin;
            maxHorizontal = horizontalMax;
            chargeTimeMax = maxChargeTime;

            var boxCollider = performer.GetComponent<BoxCollider2D>();

            if (!boxCollider.enabled)
                boxCollider.enabled = true;

            var boxColliderSize = new Vector2(boxCollider.bounds.extents.x, boxCollider.bounds.extents.y);
            boxCollider.enabled = false;


            var timeTick = ( Time.fixedDeltaTime / Physics2D.velocityIterations ) * 10;
            jumperDTO = new JumperDTO()
            {
                ColliderSize = boxColliderSize,
                GravityAcceleration = Physics2D.gravity * performerRigidbody.gravityScale * ( MathHelper.Pow(timeTick, 2) ),
                Drag = 1f - timeTick * performerRigidbody.drag,
                Mass = performerRigidbody.mass,
                TimeTick = timeTick
            };
        }

        public override List<AgentActionDTO> GetPossibleActions(Vector3 position)
        {
            var reacheablePositions = new List<AgentActionDTO>();
            var trajectories = GetAllPossibleTrajestories(position);

            foreach (var item in trajectories)
            {
                if (IsWalkable(item.EndPosition))
                {
                    var action = new AgentActionDTO(position, item.EndPosition, $"{item.MotionDirection.x}:{item.MotionDirection.y}", 50, PerformAgentActionAsync, PrintAgentActionAsync);
                    reacheablePositions.Add(action);
                }
            }

            return reacheablePositions;
        }

        public override async Task PerformAgentActionAsync(AgentActionDTO action)
        {
            var jumpDirection = MathHelper.GetVector3FromString(action.ActionParameters);
            performerRigidbody.AddForce(jumpDirection * 50);

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
            var trajectory = JumpHelper.GetTrajectory(jumperDTO ,action.StartPosition, MathHelper.GetVector3FromString(action.ActionParameters));
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

            if (JumpHelper.CheckIfStaysOnGround(performer))
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

            var jumpVector = JumpHelper.GetJumpVector(chargeJumpDirection, minVertical + additionVertical, minHorizontal + additionHorizontal);
            performerRigidbody.AddForce(jumpVector);

            isPerforming = false;
            //var verticalForce = maxVertical * percent < minVertical ? maxVertical * percent : minVertical;
            //var horiziontalForce = maxHorizontal * percent < minHorizontal ? maxHorizontal * percent : minHorizontal;
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
            var adjustment = new Vector2(maxHorizontal * 0.1f, maxVertical * 0.1f);

            while (power.x > minHorizontal && power.y > minVertical)
            {
                trajectories.Add(JumpHelper.GetTrajectory(jumperDTO, startPosition, JumpHelper.GetJumpVector(jumpDirection, power.y, power.x)));
                power = power - adjustment;
            }

            return trajectories;
        }
    }
    #endregion
}

