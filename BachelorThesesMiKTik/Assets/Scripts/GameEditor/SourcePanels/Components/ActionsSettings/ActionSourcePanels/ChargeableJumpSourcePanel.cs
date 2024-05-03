using Assets.Core.GameEditor.AIActions;
using Assets.Core.GameEditor.DTOS.Action;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.GameEditor.SourcePanels.Components.ActionsSettings
{
    public class ChargeableJumpSourcePanel : ActionSourcePanelController
    {
        [SerializeField] TMP_InputField VerticalForceMin;
        [SerializeField] TMP_InputField VerticalForceMax;
        [SerializeField] TMP_InputField HorizontalForceMin;
        [SerializeField] TMP_InputField HorizontalForceMax;
        [SerializeField] TMP_InputField JumpTimeCap;

        /// <summary>
        /// Retrievs data from source panel and saves them to corresponding ActionDTO.
        /// If there is any parsing error or value should be positive, default value is used.
        /// </summary>
        /// <returns></returns>
        public override ActionDTO GetAction()
        {
            float verticalForceMin = 1;
            if (TryParse(VerticalForceMin.text, out var vertMin))
                verticalForceMin = vertMin;
            else
                OutputManager.Instance.ShowMessage("Move action parsing error! Vertical min force was setted to 1");

            float verticalForceMax = 1;
            if (TryParse(VerticalForceMax.text, out var vertMax))
                verticalForceMax = vertMax;
            else
                OutputManager.Instance.ShowMessage("Move action parsing error! Vertical max force was setted to 1");

            float horizontalForceMin = 1;
            if (TryParse(HorizontalForceMin.text, out var horMin))
                horizontalForceMin = horMin;
            else
                OutputManager.Instance.ShowMessage("Move action parsing error! Horizontal min force was setted to 1");

            float horizontalForceMax = 1;
            if (TryParse(HorizontalForceMax.text, out var horMax))
                horizontalForceMax = horMax;
            else
                OutputManager.Instance.ShowMessage("Move action parsing error! Horizontal max force was setted to 1");

            float jumpTimeCap = 1;
            if (TryParse(JumpTimeCap.text, out var jumpTime))
            {
                if (jumpTime <= 0)
                {
                    OutputManager.Instance.ShowMessage("Invalid max time setting! Setted to default = 1.");
                }
                else
                {
                    jumpTimeCap = jumpTime;
                }
            }

            if (verticalForceMin > verticalForceMax)
            {
                verticalForceMax = verticalForceMin;
                OutputManager.Instance.ShowMessage("Invalid setting of vertical min and max. Min > Max! Setted to default.");
            }

            if (horizontalForceMin > horizontalForceMax)
            {
                horizontalForceMax = horizontalForceMin;
                OutputManager.Instance.ShowMessage("Invalid setting of horizontal min and max. Min > Max! Setted to default.");
            }

            return new ChargeJumpDTO(verticalForceMin, verticalForceMax, horizontalForceMin, horizontalForceMax, jumpTimeCap);

        }

        /// <summary>
        /// If given action corresponds to source panel type, sets components of panel to
        /// to show data from ActionDTO
        /// </summary>
        /// <param name="action"></param>
        public override void SetAction(ActionDTO action)
        {
            if (( action is ChargeJumpDTO ))
            {
                ChargeJumpDTO jumpAction = (ChargeJumpDTO) action;
                VerticalForceMin.text = jumpAction.VerticalForceMin.ToString();
                VerticalForceMax.text = jumpAction.VerticalForceMax.ToString();
                HorizontalForceMin.text = jumpAction.HorizontalForceMin.ToString();
                HorizontalForceMax.text = jumpAction.HorizontalForceMax.ToString();
                JumpTimeCap.text = jumpAction.JumpTimeCap.ToString();
                return;
            }
            OutputManager.Instance.ShowMessage("Chargeable jump action panel faield to set stored action setting! Parsing error", "ObjectCreate");
        }

        /// <summary>
        /// Returs string representation of all posible actions of action represented by this source panel.
        /// </summary>
        /// <returns></returns>
        public override List<string> GetActionTypes()
        {
            return ChargeableJumpAction.ActionTypes;
        }
    }
}
