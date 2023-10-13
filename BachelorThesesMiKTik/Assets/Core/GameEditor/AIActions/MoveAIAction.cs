using Assets.Core.GameEditor.DTOS;
using Assets.Scenes.GameEditor.Core.AIActions;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameEditor.AI
{
    public class MoveAIAction : AIActionBase
    {
        private GameObject performer;

        public MoveAIAction(MapCanvasController controller, GameObject gameObject) : base(controller)
        {
            performer = gameObject;
        }

        public override List<AgentActionDTO> GetPossibleActions(Vector3 position)
        {
            var cellSize = context.GridLayout.cellSize;

            var reacheablePositions = new List<AgentActionDTO>();
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

            for (int i = 0; i < newPositions.Length; i++)
            {
                if (IsWalkable(newPositions[i]))
                {
                    reacheablePositions.Add(new AgentActionDTO(position, newPositions[i], newPositionsParams[i], 1f, PerformAction, PrintAction));
                }
            }

            return reacheablePositions;
        }

        public override void PerformAction(AgentActionDTO action)
        {
            performer.transform.position = GetPositionFromParam(action.StartPosition, action.PositionActionParameter);
        }

        public override List<GameObject> PrintAction(AgentActionDTO action)
        { 
            return new List<GameObject>() { context.CreateMarkAtPosition(action.StartPosition) };
        }

        private Vector3 GetPositionFromParam(Vector3 position, string param)
        {
            var cellSize = context.GridLayout.cellSize;

            switch (param)
            {
                case "M;1:0" : return new Vector3(position.x + cellSize.x, position.y); //Right
                case "M;1:-1": return new Vector3(position.x + cellSize.x, position.y - cellSize.y); //LowerRight
                case "M;1:1" : return new Vector3(position.x + cellSize.x, position.y + cellSize.y); //UpperRight

                case "M;-1:0": return new Vector3(position.x - cellSize.x, position.y); //Left
                case "M;-1:-1": return new Vector3(position.x - cellSize.x, position.y - cellSize.y); //LowerLeft;
                case "M;-1:1": return new Vector3(position.x - cellSize.x, position.y - cellSize.y); //UpperLeft;
            }
            return position;
    }
}
}
