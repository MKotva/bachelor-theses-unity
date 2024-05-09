using Assets.Core.GameEditor;
using Assets.Core.GameEditor.DTOS.Action;
using Assets.Scripts.GameEditor.AI;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameEditor.SourcePanels.Components.ActionsSettings
{
    public class MoveActionSourcePanel : ActionSourcePanelController
    {
        [SerializeField] TMP_InputField Speed;
        [SerializeField] TMP_InputField SpeedCap;
        [SerializeField] Toggle GroundedOnly;

        /// <summary>
        /// Retrievs data from source panel and saves them to corresponding ActionDTO.
        /// If there is any parsing error or value should be positive, default value is used.
        /// </summary>
        /// <returns></returns>
        public override ActionDTO GetAction()
        {
            var speed = MathHelper.GetPositiveFloat(Speed.text, 0, "move speed");
            var speedCap = MathHelper.GetPositiveFloat(SpeedCap.text, 0, "speed cap");

            return new MoveActionDTO(speed, speedCap, GroundedOnly.isOn);
        }

        /// <summary>
        /// If given action corresponds to source panel type, sets components of panel to
        /// to show data from ActionDTO
        /// </summary>
        /// <param name="action"></param>
        public override void SetAction(ActionDTO action)
        {
            if (( action is MoveActionDTO ))
            {
                MoveActionDTO moveAction = (MoveActionDTO) action;
                Speed.text = moveAction.Speed.ToString();
                SpeedCap.text = moveAction.SpeedCap.ToString();
                GroundedOnly.isOn = moveAction.OnlyGrounded;
                return;
            }
            OutputManager.Instance.ShowMessage("Move action panel faield to set stored action setting! Parsing error", "ObjectCreate");
        }

        /// <summary>
        /// Returs string representation of all posible actions of action represented by this source panel.
        /// </summary>
        /// <returns></returns>
        public override List<string> GetActionTypes()
        {
            return MoveAction.ActionTypes;
        }
    }
}
