using Assets.Core.GameEditor;
using Assets.Core.GameEditor.AIActions;
using Assets.Core.GameEditor.DTOS.Action;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.GameEditor.SourcePanels.Components.ActionsSettings
{
    public class FlyActionSourcePanel : ActionSourcePanelController
    {
        [SerializeField] TMP_InputField Speed;
        [SerializeField] TMP_InputField SpeedCap;

        /// <summary>
        /// Retrievs data from source panel and saves them to corresponding ActionDTO.
        /// If there is any parsing error or value should be positive, default value is used.
        /// </summary>
        /// <returns></returns>
        public override ActionDTO GetAction()
        {
            var speed = MathHelper.GetPositiveFloat(Speed.text, 0, "fly speed");
            var speedCap = MathHelper.GetPositiveFloat(SpeedCap.text, 0, "fly speed cap");
            return new FlyActionDTO(speed, speedCap);
        }

        /// <summary>
        /// If given action corresponds to source panel type, sets components of panel to
        /// to show data from ActionDTO
        /// </summary>
        /// <param name="action"></param>
        public override void SetAction(ActionDTO action)
        {
            if (action is FlyActionDTO)
            {
                var flyAction = (FlyActionDTO) action;
                Speed.text = flyAction.Speed.ToString();
                SpeedCap.text = flyAction.SpeedCap.ToString();
            }
            OutputManager.Instance.ShowMessage("Fly action panel faield to set stored action setting! Parsing error", "ObjectCreate");
        }

        /// <summary>
        /// Returs string representation of all posible actions of action represented by this source panel.
        /// </summary>
        /// <returns></returns>
        public override List<string> GetActionTypes()
        {
            return FlyAction.ActionTypes;
        }
    }
}
