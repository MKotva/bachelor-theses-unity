using Assets.Core.GameEditor.DTOS.Components;
using Assets.Scripts.GameEditor.ItemView;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.GameEditor.SourcePanels.Components
{
    public class GeneralSettingComponentController : ObjectComponent
    {
        [SerializeField] TMP_InputField NameField;
        [SerializeField] TMP_InputField GroupField;
        [SerializeField] TMP_Dropdown GroupDropDown;

        public override void SetComponent(ComponentDTO component)
        {
            if (component is GeneralSettingComponentDTO)
            {
                var general = (GeneralSettingComponentDTO) component;
                NameField.text = general.Name;
                SetGroup(general.Group);
            }
            else
            {
                InfoPanelController.Instance.ShowMessage("General component parsing error!");
            }
        }

        public override async Task<ComponentDTO> GetComponent()
        {
            return await Task.Run(() => CreateComponent());
        }

        #region PRIVATE
        private void Awake()
        {
            var groupNames = new List<string>();
            foreach (var groupPair in GameItemController.Instance.GroupViews)
            {
                groupNames.Add(groupPair.Key);
            }

            GroupDropDown.AddOptions(groupNames);
        }

        private void SetGroup(string groupName)
        {
            for (int i = 0; i < GroupDropDown.options.Count; i++)
            {
                if (GroupDropDown.options[i].text == groupName)
                {
                    GroupDropDown.value = i;
                    return;
                }
            }
            GroupField.text = groupName;
        }

        private ComponentDTO CreateComponent()
        {
            var name = NameField.text;
            if (name == "")
            {
                InfoPanelController.Instance.ShowMessage($"Invalid item name, name is empty!");
                return null;
            }

            string groupName = "";
            if (GroupDropDown.value != 0)
            {
                groupName = GroupDropDown.options[GroupDropDown.value].text;
            }
            else
            {
                if (GroupField.text == "")
                {
                    InfoPanelController.Instance.ShowMessage($"Invalid group name, name is empty or Group! Please select or create group!");
                    return null;
                }
                groupName = GroupField.text;
            }
            return new GeneralSettingComponentDTO(name, groupName);
        }
        #endregion
    }
}
