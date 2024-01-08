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
        private event ActionChange actionChange;

        [SerializeField] TMP_Dropdown ActionSelection;
        [SerializeField] List<ActionSourcePanelController> SettingPanels;

        public ActionSourcePanelController ActualPanel { get; private set; }

        public void AddListener(ActionChange handler)
        {
            actionChange += handler;
        }

        public void SetAction(ActionDTO action)
        {
            for(int i = 0; i < ActionSelection.options.Count; i++) 
            {
                if (ActionSelection.options[i].text != action.Name)
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

        public ActionDTO GetAction()
        {
            var dto = ActualPanel.GetAction();
            dto.Name = ActionSelection.options[ActionSelection.value].text;
            return dto;
        }

        #region PRIVATE
        private void Awake()
        {
            ActualPanel = SettingPanels.First();
            ActionSelection.onValueChanged.AddListener(delegate { OnActionChange(); });
        }

        private void OnActionChange()
        {
            ActualPanel.gameObject.SetActive(false);
            ActualPanel = SettingPanels[ActionSelection.value];
            if(actionChange != null) 
                actionChange.Invoke(ActualPanel.GetActionTypes());
            ActualPanel.gameObject.SetActive(true);
        }

        #endregion
    }
}
