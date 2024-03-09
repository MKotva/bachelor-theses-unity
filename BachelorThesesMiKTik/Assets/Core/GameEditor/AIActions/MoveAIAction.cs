using Assets.Core.GameEditor.DTOS;
using Assets.Scenes.GameEditor.Core.AIActions;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.GameEditor.AI
{
    public class MoveAIAction : AIActionBase
    {
        private static List<string> actionTypes = new List<string>
        {
            "Move left",
            "Move right",
        };
        public static List<string> ActionTypes
        {
            get
            {
                return new List<string>(actionTypes);
            }
        }

        private GameObject performer;
        private float speed;
        private float speedCap;
        private bool canFall;

        public MoveAIAction(GameObject gameObject, float moveSpeed = 1, float moveSpeedCap = 1, bool canFallOf = false)
        {
            performer = gameObject;
            speed = moveSpeed;
            speedCap = moveSpeedCap;
            canFall = canFallOf;
        }

        public override List<AgentActionDTO> GetPossibleActions(Vector3 position)
        {
            var cellSize = map.GridLayout.cellSize;

            var reacheablePositions = new List<AgentActionDTO>();
            var newPositions = new Vector3[]
            {
                map.GetCellCenterPosition(new Vector3(position.x + cellSize.x, position.y)), //Right
                map.GetCellCenterPosition(new Vector3(position.x + cellSize.x, position.y - cellSize.y)), //LowerRight
                map.GetCellCenterPosition(new Vector3(position.x + cellSize.x, position.y + cellSize.y)), //UpperRight

                map.GetCellCenterPosition(new Vector3(position.x - cellSize.x, position.y)), //Left
                map.GetCellCenterPosition(new Vector3(position.x - cellSize.x, position.y - cellSize.y)), //LowerLeft;
                map.GetCellCenterPosition(new Vector3(position.x - cellSize.x, position.y - cellSize.y)) //UpperLeft;
            };

            var newPositionsParams = new string[]
            {
                "M;1:0", //Right
                "M;1:-1", //LowerRight
                "M;1:1", //UpperRight

                "M;-1:0", //Left
                "M;-1:-1", //LowerLeft;
                "M;-1:1" //UpperLeft;
            };

            for (int i = 0; i < newPositions.Length; i++)
            {
                if (IsWalkable(newPositions[i]))
                {
                    reacheablePositions.Add(new AgentActionDTO(position, newPositions[i], newPositionsParams[i], 1f, PerformActionAsync, PrintActionAsync));
                }
            }

            return reacheablePositions;
        }

        public override async Task PerformActionAsync(AgentActionDTO action)
        {
            performer.transform.position = GetPositionFromParam(action.StartPosition, action.PositionActionParameter);
            await Task.Delay(1000);
        }

        public override async Task<List<GameObject>> PrintActionAsync(AgentActionDTO action)
        {
            var result = new List<GameObject>() { map.Marker.CreateMarkAtPosition(action.StartPosition) };
            return await Task.FromResult(result);
        }

        private Vector3 GetPositionFromParam(Vector3 position, string param)
        {
            var cellSize = map.GridLayout.cellSize;

            switch (param)
            {
                case "M;1:0" : return new Vector3(position.x + cellSize.x, position.y); //Right
                case "M;1:-1": return new Vector3(position.x + cellSize.x, position.y - cellSize.y); //LowerRight
                case "M;1:1" : return new Vector3(position.x + cellSize.x, position.y + cellSize.y); //UpperRight

                case "M;-1:0": return new Vector3(position.x - cellSize.x, position.y); //Left
                case "M;-1:-1": return new Vector3(position.x - cellSize.x, position.y - cellSize.y); //LowerLeft;
                case "M;-1:1": return new Vector3(position.x - cellSize.x, position.y - cellSize.y); //UpperLeft;
            }
            return map.GetCellCenterPosition(position);
    }

        public override bool IsPerforming()
        {
            throw new System.NotImplementedException();
        }

        public override void PerformAction(string action)
        {
            throw new System.NotImplementedException();
        }
    }
}
