using Assets.Core.GameEditor;
using Assets.Core.GameEditor.DTOS.Action;
using Assets.Scripts.GameEditor.AI;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameEditor.SourcePanels.Components.ActionsSettings
{
    public class SimpleJumpActionSourcePanel : ActionSourcePanelController
    {
        [SerializeField] Toggle GroundedOnly;
        [SerializeField] TMP_InputField VerticalForce;
        [SerializeField] TMP_InputField HorizontalForce;
        [SerializeField] TMP_InputField SpeedCap;
        /// <summary>
        /// Retrievs data from source panel and saves them to corresponding ActionDTO.
        /// If there is any parsing error or value should be positive, default value is used.
        /// </summary>
        /// <returns></returns>
        public override ActionDTO GetAction()
        {
            float verticalForce = MathHelper.GetPositiveFloat(VerticalForce.text, 2, "vertical force");
            float horizontalForce = MathHelper.GetPositiveFloat(HorizontalForce.text, 2, "horizontal force");
            float speedCap = MathHelper.GetPositiveFloat(SpeedCap.text, 5, "Speed cap");

            return new SimpleJumpActionDTO(verticalForce, horizontalForce, speedCap, GroundedOnly.isOn);
        }

        /// <summary>
        /// If given action corresponds to source panel type, sets components of panel to
        /// to show data from ActionDTO
        /// </summary>
        /// <param name="action"></param>
        public override void SetAction(ActionDTO action)
        {
            if (( action is SimpleJumpActionDTO ))
            {
                SimpleJumpActionDTO simpleJumpAction = (SimpleJumpActionDTO) action;
                VerticalForce.text = simpleJumpAction.VerticalForce.ToString();
                HorizontalForce.text = simpleJumpAction.HorizontalForce.ToString();
                SpeedCap.text = simpleJumpAction.SpeedCap.ToString();
                GroundedOnly.isOn = simpleJumpAction.OnlyGrounded;
                return;
            }
            OutputManager.Instance.ShowMessage("Simple jump action panel faield to set stored action setting! Parsing error", "ObjectCreate");
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
