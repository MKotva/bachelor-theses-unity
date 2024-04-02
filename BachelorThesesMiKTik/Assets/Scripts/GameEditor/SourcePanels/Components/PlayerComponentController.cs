using Assets.Core.GameEditor.Components;
using Assets.Scripts.GameEditor.SourcePanels.Components;
using Assets.Scripts.GameEditor.SourcePanels.Components.ActionsSettings;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameEditor.SourcePanels
{
    public class PlayerComponentController : ObjectComponent
    {
        [SerializeField] ActionsSettingController ActionSettings;
        [SerializeField] RuntimeActionSettingPanelController CreateController;
        [SerializeField] RuntimeActionSettingPanelController UpdateController;
        [SerializeField] BindingSettingController BindingSettings;

        private bool isInitialized = false;

        public override void SetComponent(CustomComponent component)
        {
            if (component is PlayerComponent)
            {
                if (!isInitialized)
                {
                    Initialize();
                }

                var player = (PlayerComponent) component;
                ActionSettings.SetAction(player.Actions);
                CreateController.SetPanel(player.OnCreateAction);
                UpdateController.SetPanel(player.OnUpdateAction);
                BindingSettings.SetBindings(player.Bindings);
            }
            else
            {
                ErrorOutputManager.Instance.ShowMessage("Player component parsing error!", "ObjectCreate");
            }
        }

        public override CustomComponent GetComponent()
        {
            return CreateComponent();
        }

        #region PRIVATE
        /// <summary>
        /// Used instead of awake thank to reasons above.
        /// </summary>
        private void Start()
        {
            if (!isInitialized)
                Initialize();
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

        private void Initialize()
        {
            OnActionChange(ActionSettings.ActualPanel.GetActionTypes());
            ActionSettings.OnActionChange += OnActionChange;
            isInitialized = true;
        }

        private PlayerComponent CreateComponent()
        {
            var action = ActionSettings.GetAction();
            var binding = BindingSettings.GetBindings();
            return new PlayerComponent(action, binding, CreateController.ActionCode, UpdateController.ActionCode);
        }
        #endregion
    }
}
