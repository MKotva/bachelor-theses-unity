using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Core.GameEditor.DTOS.Components
{
    [Serializable]
    public abstract class ComponentDTO
    {
        public string ComponentName;
        public int ComponentID;
        public abstract Task Set(ItemData item);
        public virtual T GetOrAddComponent<T>(GameObject ob) where T : Component
        {
            T component;
            if (!ob.TryGetComponent<T>(out component))
                component = ob.AddComponent<T>();
            return component;
        }
    }
}
