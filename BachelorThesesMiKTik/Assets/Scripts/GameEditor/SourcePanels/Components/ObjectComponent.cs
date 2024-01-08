using Assets.Core.GameEditor.DTOS.Components;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.GameEditor.SourcePanels.Components
{
    public abstract class ObjectComponent: MonoBehaviour
    {
        internal ExitMethod callback;

        public delegate void ExitMethod(string componentName);
        public abstract void SetComponent(ComponentDTO component);
        public abstract Task<ComponentDTO> GetComponent();
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
