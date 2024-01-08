using Assets.Core.GameEditor.DTOS.Components;
using Assets.Scripts.GameEditor.SourcePanels.Components.ActionsSettings;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.GameEditor.SourcePanels.Components
{
    public class AIComponentController : ObjectComponent
    {
        [SerializeField] ActionsSettingController ActionPanel;
        [SerializeField] RuntimeActionSettingPanelController CreateController;
        [SerializeField] RuntimeActionSettingPanelController UpdateController;

        public override void SetComponent(ComponentDTO component)
        {
            if (component is AIComponentDTO)
            {
                var aiComponent = (AIComponentDTO) component;
                ActionPanel.SetAction(aiComponent.Action);
                CreateController.SetPanel(aiComponent.OnCreateAction);
                UpdateController.SetPanel(aiComponent.OnUpdateAction);
            }
            else
            {
                InfoPanelController.Instance.ShowMessage("ObjectCreate", "AI component parsing error!");
            }
        }

        public override async Task<ComponentDTO> GetComponent()
        {
            return await Task.Run(() => CreateComponent());
        }

        private AIComponentDTO CreateComponent()
        {
            var action = ActionPanel.GetAction();
            return new AIComponentDTO(action, CreateController.ActionCode, UpdateController.ActionCode);
        }
    }
}
