using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.GameEditor.SourcePanels.Components
{
    public abstract class ObjectComponent: MonoBehaviour
    {
        internal ExitMethod callback;

        public delegate void ExitMethod(string componentName);
        public abstract Task SetItem(ItemData item);
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
