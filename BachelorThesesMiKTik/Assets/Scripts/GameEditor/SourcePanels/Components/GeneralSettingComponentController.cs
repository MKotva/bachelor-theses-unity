using Assets.Core.GameEditor.Components;
using Assets.Scripts.GameEditor.Managers;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.GameEditor.SourcePanels.Components
{
    public class GeneralSettingComponentController : ObjectComponent
    {
        [SerializeField] TMP_InputField NameField;
        [SerializeField] TMP_InputField GroupField;
        [SerializeField] TMP_Dropdown GroupDropDown;

        public override void SetComponent(CustomComponent component)
        {
            if (component is GeneralSettingComponent)
            {
                var general = (GeneralSettingComponent) component;
                NameField.text = general.Name;
                SetGroup(general.Group);
            }
            else
            {
                ErrorOutputManager.Instance.ShowMessage("General component parsing error!","ObjectCreate");
            }
        }

        public override CustomComponent GetComponent()
        {
            return CreateComponent();
        }

        #region PRIVATE
        private void Awake()
        {
            var groupNames = new List<string>();
            foreach (var groupPair in ItemManager.Instance.Groups)
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

        private CustomComponent CreateComponent()
        {
            var name = NameField.text;
            if (name == "")
            {
                ErrorOutputManager.Instance.ShowMessage($"Invalid item name, name is empty!", "ObjectCreate");
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
                    ErrorOutputManager.Instance.ShowMessage($"Invalid group name, name is empty or Group! Please select or create group!", "ObjectCreate");
                    return null;
                }
                groupName = GroupField.text;
            }
            return new GeneralSettingComponent(name, groupName);
        }
        #endregion
    }
}
