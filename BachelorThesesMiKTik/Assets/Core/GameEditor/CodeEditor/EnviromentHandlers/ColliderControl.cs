using Assets.Core.GameEditor.Attributes;
using Assets.Core.GameEditor.CodeEditor.EnviromentObjects;
using Assets.Core.SimpleCompiler.Exceptions;
using Assets.Scripts.GameEditor.Entiti;
using UnityEngine;
using static DG.Tweening.DOTweenModuleUtils;

namespace Assets.Core.GameEditor.CodeEditor.EnviromentHandlers
{
    public class ColliderControl : EnviromentObject
    {
        ColliderController collider;

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
            if(collider == null)
                throw new RuntimeException($"\"Exception in method \\\"ChangePhysics\\\"! Object is missing Collision Component!");

            if(collider.ObjectCollider == null)
                throw new RuntimeException($"\"Exception in method \\\"ChangePhysics\\\"! Object is missing Collision Component!");

            collider.ObjectCollider.isTrigger = isOn;
        }

        [CodeEditorAttribute("This method will try to find given hit normal. " +
            "THIS WORKS ONLY FOR OBJECTS WITH ENABLED PHYSICAL!!! It can be usefull for detecting direction of hit" 
            , "(num xNormal, num yNormal)")]
        public bool HasCollidedInDirection(float x, float y)
        {
            if (collider == null)
                throw new RuntimeException($"\"Exception in method \\\"ChangePhysics\\\"! Object is missing Collision Component!");

            return collider.ContainsContactPoint(new Vector2(x, y));
        }

    }
}
