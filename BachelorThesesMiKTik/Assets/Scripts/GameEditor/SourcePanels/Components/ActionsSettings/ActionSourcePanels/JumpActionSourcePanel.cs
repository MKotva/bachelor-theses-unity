using Assets.Core.GameEditor.DTOS.Action;
using Assets.Scripts.GameEditor.AI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameEditor.SourcePanels.Components.ActionsSettings
{
    public class JumpActionSourcePanel : ActionSourcePanelController
    {
        [SerializeField] Toggle IsChargeable;
        [SerializeField] Toggle GroundedOnly;
        [SerializeField] SimpleJumpActionSourcePanel SimpleJump;
        [SerializeField] ChargeableJumpSourcePanel ChargeableJump;

        private ActionSourcePanelController active;

        private void Awake()
        {
            active = ChargeableJump;
            IsChargeable.onValueChanged.AddListener(delegate { OnValueChange(); });
        }

        private void OnValueChange()
        {
            active.gameObject.SetActive(false);
            if(IsChargeable.isOn) 
            {
                active = ChargeableJump;
            }
            else
            {
                active = SimpleJump;
            }
            active.gameObject.SetActive(true);
        }

        /// <summary>
        /// Retrievs data from source panel and saves them to corresponding ActionDTO.
        /// If there is any parsing error or value should be positive, default value is used.
        /// </summary>
        /// <returns></returns>
        public override ActionDTO GetAction()
        {
            return active.GetAction();
        }

        /// <summary>
        /// If given action corresponds to source panel type, sets components of panel to
        /// to show data from ActionDTO
        /// </summary>
        /// <param name="action"></param>
        public override void SetAction(ActionDTO action)
        {
            if(action is SimpleJumpActionDTO)
            {
                SimpleJump.SetAction(action);
                IsChargeable.isOn = false;
            }
            else if(action is ChargeJumpDTO) 
            {
                ChargeableJump.SetAction(action);
                IsChargeable.isOn = true;
            }
        }

        /// <summary>
        /// Returs string representation of all posible actions of action represented by this source panel.
        /// </summary>
        /// <returns></returns>
        public override List<string> GetActionTypes()
        {
            return JumpAction.ActionTypes;
        }
    }
}
