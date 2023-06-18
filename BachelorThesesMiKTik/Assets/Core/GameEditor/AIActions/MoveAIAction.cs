using Assets.Core.GameEditor.DTOS;
using Assets.Scenes.GameEditor.Core.AIActions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.GameEditor.AI
{
    public class MoveAIAction : AIActionBase
    {
        public MoveAIAction(MapCanvasController controller) : base(controller) { }

        public override List<AgentActionDTO> GetReacheablePosition(Vector3 position)
        {
            var reacheablePositions = new List<AgentActionDTO>();
            var cellSize = context.GridLayout.cellSize;

            var newPositions = new Vector3[]
            {
                context.GetCellCenterPosition(new Vector3(position.x + cellSize.x, position.y)), //Right
                context.GetCellCenterPosition(new Vector3(position.x + cellSize.x, position.y - cellSize.y)), //LowerRight
                context.GetCellCenterPosition(new Vector3(position.x + cellSize.x, position.y + cellSize.y)), //UpperRight

                context.GetCellCenterPosition(new Vector3(position.x - cellSize.x, position.y)), //Left
                context.GetCellCenterPosition(new Vector3(position.x - cellSize.x, position.y - cellSize.y)), //LowerLeft;
                context.GetCellCenterPosition(new Vector3(position.x - cellSize.x, position.y - cellSize.y)) //UpperLeft;
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

            for(int i = 0; i < newPositions.Length; i++)
            {
                if(IsWalkable(newPositions[i]))
                {
                    reacheablePositions.Add(new AgentActionDTO(position, newPositions[i], newPositionsParams[i], 1f, PerformAction, PerformActionWithPrint));
                }
            }

            return reacheablePositions;
        }

        public override void PerformAction(Vector3 startPosition, string parameters)
        {
            //context.CreateMarkAtPosition(startPosition);
        }

        public override List<GameObject> PerformActionWithPrint(Vector3 startPosition, string parameters)
        {
            return new List<GameObject>() { context.CreateMarkAtPosition(startPosition) };
        }
    }
}
