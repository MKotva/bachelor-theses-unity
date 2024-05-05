using System;
using System.Threading.Tasks;
using Assets.Core.GameEditor.DTOS.Action;
using Assets.Core.SimpleCompiler;
using Assets.Scripts.GameEditor.AI;
using UnityEngine;

namespace Assets.Core.GameEditor.Components
{
    [Serializable]
    public class AIComponent : CustomComponent
    {
        public ActionDTO Action;
        public SimpleCode OnCreateAction;
        public SimpleCode OnUpdateAction;

        public AIComponent() {}

        public AIComponent(ActionDTO action, SimpleCode createAction, SimpleCode updateAction) 
        {
            ComponentName = "AI Control";
            Action = action;
            OnCreateAction = createAction;
            OnUpdateAction = updateAction;
        }

        public override async Task Initialize()
        {
            if(OnCreateAction != null) 
            {
                await OnCreateAction.CompileAsync();
            }

            if (OnUpdateAction != null)
            {
                await OnUpdateAction.CompileAsync();
            }
        }

        public override void Set(ItemData item) 
        {
            foreach(var component in item.Components)
            {
                if (component.ComponentName == "Physics")
                    return;
            }

            var physicsComponent = new PhysicsComponent();
            physicsComponent.Set(item);
            item.Components.Add(physicsComponent);
        }

        public override void SetInstance(ItemData item, GameObject instance)
        {
            var agent = GetOrAddComponent<AIObjectController>(instance);
            agent.Initialize(this);
        }
    }
}
