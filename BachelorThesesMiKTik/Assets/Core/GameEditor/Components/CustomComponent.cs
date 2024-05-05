using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Core.GameEditor.Components
{
    [Serializable]
    public abstract class CustomComponent
    {
        public string ComponentName;
        public int ComponentID;
        public abstract Task Initialize();
        public abstract void Set(ItemData item);
        public abstract void SetInstance(ItemData item, GameObject instance);
        public virtual T GetOrAddComponent<T>(GameObject ob) where T : Component
        {
            T component;
            if (!ob.TryGetComponent<T>(out component))
                component = ob.AddComponent<T>();
            return component;
        }
    }
}
