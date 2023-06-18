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

        public override AgentActionDTO GetReacheablePosition(Vector3 position)
        {
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

            var reacheablePositions = new List<Vector3>();
            var actionParameters = new List<string>();
            foreach(var possiblePos in reacheablePositions)
            {
                if(IsWalkable(possiblePos))
                {
                    reacheablePositions.Add(possiblePos);
                }
            }

            return new AgentActionDTO(reacheablePositions, actionParameters, PerformAction, 1f);
        }

        public override void PerformAction(string parameters)
        {
            throw new NotImplementedException();
        }
    }
}
