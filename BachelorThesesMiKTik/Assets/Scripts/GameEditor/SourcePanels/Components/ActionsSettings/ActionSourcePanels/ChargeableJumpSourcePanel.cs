using Assets.Core.GameEditor;
using Assets.Core.GameEditor.AIActions;
using Assets.Core.GameEditor.DTOS.Action;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameEditor.SourcePanels.Components.ActionsSettings
{
    public class ChargeableJumpSourcePanel : ActionSourcePanelController
    {
        [SerializeField] Toggle GroundedOnly;
        [SerializeField] TMP_InputField VerticalForceMin;
        [SerializeField] TMP_InputField VerticalForceMax;
        [SerializeField] TMP_InputField HorizontalForceMin;
        [SerializeField] TMP_InputField HorizontalForceMax;
        [SerializeField] TMP_InputField JumpTimeCap;
        [SerializeField] TMP_InputField SpeedCap;

        /// <summary>
        /// Retrievs data from source panel and saves them to corresponding ActionDTO.
        /// If there is any parsing error or value should be positive, default value is used.
        /// </summary>
        /// <returns></returns>
        public override ActionDTO GetAction()
        {
            float verticalForceMin = MathHelper.GetPositiveFloat(VerticalForceMin.text, 0, "vertical min force");
            float verticalForceMax = MathHelper.GetPositiveFloat(VerticalForceMax.text, 0, "vertical max force");
            float horizontalForceMin = MathHelper.GetPositiveFloat(HorizontalForceMin.text, 0, "horizontal min force");
            float horizontalForceMax = MathHelper.GetPositiveFloat(HorizontalForceMax.text, 0, "horizontal max force");
            float jumpTimeCap = MathHelper.GetPositiveFloat(JumpTimeCap.text, 0, "jump max time setting");
            float jumpSpeedCap = MathHelper.GetPositiveFloat(SpeedCap.text, 0, "jump speed cap");

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

            return new ChargeJumpDTO(verticalForceMin, verticalForceMax, horizontalForceMin, horizontalForceMax, jumpTimeCap, jumpSpeedCap, GroundedOnly.isOn);

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
                SpeedCap.text = jumpAction.JumpSpeedCap.ToString();
                GroundedOnly.isOn = jumpAction.OnlyGrounded;
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
