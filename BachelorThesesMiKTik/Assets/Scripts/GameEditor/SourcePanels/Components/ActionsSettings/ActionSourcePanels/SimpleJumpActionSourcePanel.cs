using Assets.Core.GameEditor.DTOS.Action;
using Assets.Scripts.GameEditor.AI;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.GameEditor.SourcePanels.Components.ActionsSettings
{
    public class SimpleJumpActionSourcePanel : ActionSourcePanelController
    {
        [SerializeField] TMP_InputField VerticalForce;
        [SerializeField] TMP_InputField HorizontalForce;
        /// <summary>
        /// Retrievs data from source panel and saves them to corresponding ActionDTO.
        /// If there is any parsing error or value should be positive, default value is used.
        /// </summary>
        /// <returns></returns>
        public override ActionDTO GetAction()
        {
            float verticalForce = 1;
            if (TryParse(VerticalForce.text, out var vertForce))
                verticalForce = vertForce;
            else
                OutputManager.Instance.ShowMessage("Move action parsing error! Vertical force was setted to 1");

            float horizontalForce = 1;
            if (TryParse(HorizontalForce.text, out var horizForce))
                horizontalForce = horizForce;
            else
                OutputManager.Instance.ShowMessage("Move action parsing error! Horizontal force was setted to 1");

            return new SimpleJumpActionDTO(verticalForce, horizontalForce);
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
