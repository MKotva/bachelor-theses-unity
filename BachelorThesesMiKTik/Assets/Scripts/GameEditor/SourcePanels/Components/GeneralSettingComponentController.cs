using Assets.Scripts.GameEditor.ItemView;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.GameEditor.SourcePanels.Components
{
    public class GeneralSettingComponentController : ObjectComponent
    {
        [SerializeField] TMP_InputField NameField;
        [SerializeField] TMP_InputField GroupField;
        [SerializeField] TMP_Dropdown GroupDropDown;

        private void Start()
        {
            var groupNames = new List<string>();
            foreach(var groupPair in GameItemController.Instance.GroupViews)
            {
                groupNames.Add(groupPair.Key);
            }

            GroupDropDown.AddOptions(groupNames);
        }

        public override async Task SetItem(ItemData item)
        {
            item.ShownName = NameField.text;
            item.Prefab.name = NameField.text;

            if(GroupDropDown.value != 0)
            {
                item.GroupName = GroupDropDown.options[GroupDropDown.value].text;
            }
            else
            {
                item.GroupName = GroupField.text;
            }
        }
    }
}
