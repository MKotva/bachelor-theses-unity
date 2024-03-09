using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Core.GameEditor.DTOS.Components
{
    [Serializable]
    public class GeneralSettingComponentDTO : ComponentDTO
    {
        public string Name;
        public string Group;

        public GeneralSettingComponentDTO() { }
        public GeneralSettingComponentDTO(string name, string group) 
        {
            ComponentName = "General Setting";
            Name = name;
            Group = group;
        }

        public override async Task Set(ItemData item)
        {
            item.ShownName = Name;
            item.Prefab.name = Name;
            item.GroupName = Group;
        }

        public override void SetInstance(ItemData item, GameObject instance) {}
    }
}
