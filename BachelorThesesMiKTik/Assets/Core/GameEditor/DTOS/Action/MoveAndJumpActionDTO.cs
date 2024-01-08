using Assets.Scenes.GameEditor.Core.AIActions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Core.GameEditor.DTOS.Action
{
    [Serializable]
    public class MoveAndJumpActionDTO : ActionDTO
    {
        public MoveActionDTO MoveAction;
        public ActionDTO JumpAction;

        public MoveAndJumpActionDTO(ActionDTO moveActionDTO, ActionDTO jumpAction)
        {
            MoveAction = (MoveActionDTO)moveActionDTO;
            JumpAction = jumpAction; // TODO: There might be a problem with the parsing.
        }

        public override List<AIActionBase> GetAction(GameObject instance)
        {
            var move = MoveAction.GetAction(instance);
            var jump = JumpAction.GetAction(instance);
            move.AddRange(jump);
            return move;
        }
    }
}
