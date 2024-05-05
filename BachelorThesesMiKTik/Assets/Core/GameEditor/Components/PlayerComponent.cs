using Assets.Core.GameEditor.DTOS.Action;
using Assets.Core.GameEditor.DTOS.SourcePanels;
using Assets.Core.SimpleCompiler;
using Assets.Scripts.GameEditor.Entiti;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Core.GameEditor.Components
{
    [Serializable]
    public class PlayerComponent : CustomComponent
    {
        public ActionDTO Actions;
        public List<ActionBindDTO> Bindings;
        public SimpleCode OnCreateAction;
        public SimpleCode OnUpdateAction;

        public PlayerComponent()
        {
            Actions = null;
            Bindings = new List<ActionBindDTO>();
            OnCreateAction = null;
            OnUpdateAction = null;
        }

        public PlayerComponent(ActionDTO action, List<ActionBindDTO> bindings, SimpleCode createAction, SimpleCode updateAction)
        {
            ComponentName = "Player Control";
            Actions = action;
            Bindings = bindings;
            OnCreateAction = createAction;
            OnUpdateAction = updateAction;
        }


        public override async Task Initialize()
        {
            var tasks = new List<Task>();
            if (OnCreateAction != null)
            {
                tasks.Add(OnCreateAction.CompileAsync());
            }

            if (OnUpdateAction != null)
            {
                tasks.Add(OnUpdateAction.CompileAsync());
            }

            foreach(var bind in Bindings) 
            {
                if(bind.ActionCode != null)
                {
                    tasks.Add(bind.ActionCode.CompileAsync());
                }
            }
            await Task.WhenAll(tasks);
        }

        public override void Set(ItemData item)
        {
            foreach (var component in item.Components)
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
            var playerController = GetOrAddComponent<PlayerObjectController>(instance);
            playerController.Initialize(this);
        }
    }
}
