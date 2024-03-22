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

        public override void Set(ItemData item) {}

        public override void SetInstance(ItemData item, GameObject instance)
        {
            var agent = GetOrAddComponent<AIAgent>(instance);
            agent.Initialize(this);
        }
    }
}
