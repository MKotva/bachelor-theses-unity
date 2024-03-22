using Assets.Core.GameEditor.Components;
using UnityEngine;

namespace Assets.Scripts.GameEditor.SourcePanels.Components
{
    public abstract class ObjectComponent: MonoBehaviour
    {
        internal ExitMethod callback;

        public delegate void ExitMethod(string componentName);
        public abstract void SetComponent(CustomComponent component);
        public abstract CustomComponent GetComponent();
        public virtual void OnExitClick()
        {
            if (callback != null)
                callback.Invoke(gameObject.name);
        }
        public virtual void SetExitMethod(ExitMethod exit)
        {
            callback = exit;
        }
    }
}
