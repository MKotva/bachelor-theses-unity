using Assets.Core.GameEditor.DTOS.Action;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.GameEditor.SourcePanels.Components
{
    public class ActionsSettingController : MonoBehaviour
    {
        public delegate void ActionChange(List<string> actions);
        public event ActionChange OnActionChange;

        [SerializeField] TMP_Dropdown ActionType;
        [SerializeField] List<ActionSourcePanelController> SettingPanels;

        public ActionSourcePanelController ActualPanel { get; private set; }

        /// <summary>
        /// This method will find correct action panel based on given ActionDTO.name and
        /// set it as active panel (and initialize panel to reflect data from ActionDTO). 
        /// </summary>
        /// <param name="action"></param>
        public void SetAction(ActionDTO action)
        {
            for(int i = 0; i < ActionType.options.Count; i++) 
            {
                if (ActionType.options[i].text != action.Name)
                    continue;
                if(i >= SettingPanels.Count)
                {
                    InfoPanelController.Instance.ShowMessage("Action setting parsing error!", "ObjectCreate");
                    return;
                }
                
                ActualPanel = SettingPanels[i];
                ActualPanel.SetAction(action);
                return;
            }
        }

        /// <summary>
        /// </summary>
        /// <returns>ActionDTO which represents actual selected action type</returns>
        public ActionDTO GetAction()
        {
            var dto = ActualPanel.GetAction();
            dto.Name = ActionType.options[ActionType.value].text;
            return dto;
        }

        #region PRIVATE
        private void Awake()
        {
            ActualPanel = SettingPanels.First();
            ActionType.onValueChanged.AddListener(delegate { ChangeAction(); });
        }

        /// <summary>
        /// This method handles the ActionType change be replacing an active ActionPanel with panel
        /// coresponding to the selected type. Then the method will load all possible action 
        /// types (string representation of all posible actions of this action type [like jump -> (up, left right)])
        /// and calls calback method (if callback has been set)
        /// 
        /// </summary>
        private void ChangeAction()
        {
            ActualPanel.gameObject.SetActive(false);
            ActualPanel = SettingPanels[ActionType.value];
            if(OnActionChange != null)
                OnActionChange.Invoke(ActualPanel.GetActionTypes());
            ActualPanel.gameObject.SetActive(true);
        }
        #endregion
    }
}
