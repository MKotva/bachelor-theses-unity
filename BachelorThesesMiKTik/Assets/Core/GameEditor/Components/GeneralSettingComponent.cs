using Assets.Scripts.GameEditor.ObjectInstancesController;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Core.GameEditor.Components
{
    [Serializable]
    public class GeneralSettingComponent : CustomComponent
    {
        public string Name;
        public string Group;

        public GeneralSettingComponent() { }
        public GeneralSettingComponent(string name, string group) 
        {
            ComponentName = "General Setting";
            Name = name;
            Group = group;
        }

        public override Task Initialize() { return Task.CompletedTask; }

        public override void Set(ItemData item)
        {
            item.ShownName = Name;
            item.Prefab.name = Name;
            item.GroupName = Group;
            item.Id = Name.GetHashCode();
        }

        public override void SetInstance(ItemData item, GameObject instance) 
        {
            instance.AddComponent<ObjectController>();
        }
    }
}
