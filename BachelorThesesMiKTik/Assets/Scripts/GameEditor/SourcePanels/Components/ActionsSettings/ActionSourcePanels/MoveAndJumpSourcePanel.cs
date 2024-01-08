using Assets.Core.GameEditor.DTOS.Action;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.GameEditor.SourcePanels.Components.ActionsSettings
{
    public class MoveAndJumpSourcePanel : ActionSourcePanelController
    {
        [SerializeField] MoveActionSourcePanel movePanel;
        [SerializeField] JumpActionSourcePanel jumpPanel;

        /// <summary>
        /// Retrievs data from source panel and saves them to corresponding ActionDTO.
        /// If there is any parsing error or value should be positive, default value is used.
        /// </summary>
        /// <returns></returns>
        public override ActionDTO GetAction()
        {
            var move = movePanel.GetAction();
            var jump = jumpPanel.GetAction(); 
            return new MoveAndJumpActionDTO(move, jump);
        }

        /// <summary>
        /// If given action corresponds to source panel type, sets components of panel to
        /// to show data from ActionDTO
        /// </summary>
        /// <param name="action"></param>
        public override void SetAction(ActionDTO actions)
        {
            movePanel.SetAction(actions);
            jumpPanel.SetAction(actions);
        }

        /// <summary>
        /// Returs string representation of all posible actions of action represented by this source panel.
        /// </summary>
        /// <returns></returns>
        public override List<string> GetActionTypes()
        {
            var moveActions = movePanel.GetActionTypes();
            var jumpActions = jumpPanel.GetActionTypes();
            return new List<string>(moveActions.Concat(jumpActions));
        }
    }
}
