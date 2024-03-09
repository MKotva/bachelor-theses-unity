using Assets.Core.GameEditor.DTOS.Components;
using Assets.Scripts.GameEditor.SourcePanels.Components;
using Assets.Scripts.GameEditor.SourcePanels.Components.ActionsSettings;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.GameEditor.SourcePanels
{
    public class PlayerComponentController : ObjectComponent
    {
        [SerializeField] ActionsSettingController ActionSettings;
        [SerializeField] RuntimeActionSettingPanelController CreateController;
        [SerializeField] RuntimeActionSettingPanelController UpdateController;
        [SerializeField] BindingSettingController BindingSettings;

        //This is needed because Awake func of this script is called sooner than in given scripts.
        private bool isInitialized = false;

        public override void SetComponent(ComponentDTO component)
        {
            if (component is PlayerComponentDTO)
            {
                var player = (PlayerComponentDTO) component;
                ActionSettings.SetAction(player.Action);
                CreateController.SetPanel(player.OnCreateAction);
                UpdateController.SetPanel(player.OnUpdateAction);
                //TODO: Check if actions are setted.
                BindingSettings.SetBindings(player.Bindings);
            }
            else
            {
                InfoPanelController.Instance.ShowMessage("Player component parsing error!");
            }
        }

        public override async Task<ComponentDTO> GetComponent()
        {
            return await Task.Run(() => CreateComponent());
        }

        #region PRIVATE
        /// <summary>
        /// Used instead of awake thank to reasons above.
        /// </summary>
        private void Update()
        {
            if (!isInitialized)
            {
                OnActionChange(ActionSettings.ActualPanel.GetActionTypes());
                ActionSettings.OnActionChange += OnActionChange;
                isInitialized = true;
            }
        }

        /// <summary>
        /// Callback function, called from ActionSetting controller.
        /// Changes avalible action for bind.
        /// </summary>
        /// <param name="actions"></param>
        private void OnActionChange(List<string> actions)
        {
            BindingSettings.SetActions(actions);
        }

        private PlayerComponentDTO CreateComponent()
        {
            var action = ActionSettings.GetAction();
            var binding = BindingSettings.GetBindings();
            return new PlayerComponentDTO(action, binding, CreateController.ActionCode, UpdateController.ActionCode);
        }
        #endregion
    }
}
