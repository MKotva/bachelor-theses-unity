using Assets.Core.GameEditor.DTOS.Action;
using Assets.Core.GameEditor.DTOS.SourcePanels;
using Assets.Core.SimpleCompiler;
using Assets.Scripts.GameEditor.Entiti;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Core.GameEditor.DTOS.Components
{
    [Serializable]
    public class PlayerComponentDTO : ComponentDTO
    {
        public ActionDTO Action;
        public List<ActionBindDTO> Bindings;
        public SimpleCode OnCreateAction;
        public SimpleCode OnUpdateAction;

        public PlayerComponentDTO() 
        {
            Action = null;
            Bindings = new List<ActionBindDTO>();
            OnCreateAction = null;
            OnUpdateAction = null;
        }

        public PlayerComponentDTO(ActionDTO action, List<ActionBindDTO> bindings, SimpleCode createAction, SimpleCode updateAction) 
        {
            ComponentName = "Player Control";
            Action = action;
            Bindings = bindings;
            OnCreateAction = createAction;
            OnUpdateAction = updateAction;
        }

        public override async Task Set(ItemData item)
        {
            GetOrAddComponent<PlayerObjectController>(item.Prefab);
        }

        public override void SetInstance(ItemData item, GameObject instance)
        {
            var playerController = GetOrAddComponent<PlayerObjectController>(instance);
            playerController.Initialize(this);
        }
    }
}
