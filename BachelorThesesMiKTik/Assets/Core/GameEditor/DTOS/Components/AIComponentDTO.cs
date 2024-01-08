using System;
using System.Threading.Tasks;
using Assets.Core.GameEditor.DTOS.Action;
using Assets.Core.SimpleCompiler;
using Assets.Scripts.GameEditor.AI;

namespace Assets.Core.GameEditor.DTOS.Components
{
    [Serializable]
    public class AIComponentDTO : ComponentDTO
    {
        public ActionDTO Action;
        public SimpleCode OnCreateAction;
        public SimpleCode OnUpdateAction;

        public AIComponentDTO() {}

        public AIComponentDTO(ActionDTO action, SimpleCode createAction, SimpleCode updateAction) 
        {
            ComponentName = "AI Control";
            Action = action;
            OnCreateAction = createAction;
            OnUpdateAction = updateAction;
        }

        public override async Task Set(ItemData item)
        {
            var agent = GetOrAddComponent<AIAgent>(item.Prefab);
            agent.Initialize(this);
        }
    }
}
