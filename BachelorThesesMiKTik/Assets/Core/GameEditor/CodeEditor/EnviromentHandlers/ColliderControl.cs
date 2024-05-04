using Assets.Core.GameEditor.Attributes;
using Assets.Core.GameEditor.CodeEditor.EnviromentObjects;
using UnityEngine;

namespace Assets.Core.GameEditor.CodeEditor.EnviromentHandlers
{
    public class ColliderControl : EnviromentObject
    {
        Collider2D collider;

        public override bool SetInstance(GameObject instance)
        {
            if(!instance.TryGetComponent(out collider)) 
            {
                return false;
            }

            return true;
        }

        [CodeEditorAttribute("This method will change handling of collision to just" +
            "perform collision code, but not react physicaly.", "(bool isOn)")]
        public void ChangePhysics(bool isOn)
        {
            if(collider != null) 
            {
                collider.isTrigger = isOn;
            }
        }
    }
}
