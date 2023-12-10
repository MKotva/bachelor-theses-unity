using Assets.Core.GameEditor.DTOS;
using Assets.Scenes.GameEditor.Core.AIActions;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.GameEditor.AI
{
    public class MoveAIAction : AIActionBase
    {
        private GameObject performer;

        public MoveAIAction(GameObject gameObject)
        {
            performer = gameObject;
        }

        public override List<AgentActionDTO> GetPossibleActions(Vector3 position)
        {
            var cellSize = editor.GridLayout.cellSize;

            var reacheablePositions = new List<AgentActionDTO>();
            var newPositions = new Vector3[]
            {
                editor.GetCellCenterPosition(new Vector3(position.x + cellSize.x, position.y)), //Right
                editor.GetCellCenterPosition(new Vector3(position.x + cellSize.x, position.y - cellSize.y)), //LowerRight
                editor.GetCellCenterPosition(new Vector3(position.x + cellSize.x, position.y + cellSize.y)), //UpperRight

                editor.GetCellCenterPosition(new Vector3(position.x - cellSize.x, position.y)), //Left
                editor.GetCellCenterPosition(new Vector3(position.x - cellSize.x, position.y - cellSize.y)), //LowerLeft;
                editor.GetCellCenterPosition(new Vector3(position.x - cellSize.x, position.y - cellSize.y)) //UpperLeft;
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
            return new List<GameObject>() { editor.CreateMarkAtPosition(action.StartPosition) };
        }

        private Vector3 GetPositionFromParam(Vector3 position, string param)
        {
            var cellSize = editor.GridLayout.cellSize;

            switch (param)
            {
                case "M;1:0" : return new Vector3(position.x + cellSize.x, position.y); //Right
                case "M;1:-1": return new Vector3(position.x + cellSize.x, position.y - cellSize.y); //LowerRight
                case "M;1:1" : return new Vector3(position.x + cellSize.x, position.y + cellSize.y); //UpperRight

                case "M;-1:0": return new Vector3(position.x - cellSize.x, position.y); //Left
                case "M;-1:-1": return new Vector3(position.x - cellSize.x, position.y - cellSize.y); //LowerLeft;
                case "M;-1:1": return new Vector3(position.x - cellSize.x, position.y - cellSize.y); //UpperLeft;
            }
            return editor.GetCellCenterPosition(position);
    }
}
}
