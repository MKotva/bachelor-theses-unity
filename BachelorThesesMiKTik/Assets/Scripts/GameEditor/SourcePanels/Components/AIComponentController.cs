using Assets.Core.GameEditor.Components;
using Assets.Scripts.GameEditor.SourcePanels.Components.ActionsSettings;
using UnityEngine;

namespace Assets.Scripts.GameEditor.SourcePanels.Components
{
    public class AIComponentController : ObjectComponent
    {
        [SerializeField] ActionsSettingController ActionPanel;
        [SerializeField] RuntimeActionSettingPanelController CreateController;
        [SerializeField] RuntimeActionSettingPanelController UpdateController;

        public override void SetComponent(CustomComponent component)
        {
            if (component is AIComponent)
            {
                var aiComponent = (AIComponent) component;
                ActionPanel.SetAction(aiComponent.Action);
                CreateController.SetPanel(aiComponent.OnCreateAction);
                UpdateController.SetPanel(aiComponent.OnUpdateAction);
            }
            else
            {
                OutputManager.Instance.ShowMessage("AI component parsing error!", "ObjectCreate");
            }
        }

        public override CustomComponent GetComponent()
        {
            return CreateComponent();
        }

        private AIComponent CreateComponent()
        {
            var action = ActionPanel.GetAction();
            return new AIComponent(action, CreateController.ActionCode, UpdateController.ActionCode);
        }
    }
}
