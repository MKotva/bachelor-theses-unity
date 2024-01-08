using Assets.Core.GameEditor.DTOS.Components;
using Assets.Core.GameEditor.DTOS.SourcePanels;
using Assets.Scenes.GameEditor.Core.AIActions;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameEditor.Entiti
{
    public class PlayerObjectController : MonoBehaviour
    {
        private PlayerComponentDTO playerSetting;
        private List<AIActionBase> actions;
        private bool IsInitDone;

        public void Initialize(PlayerComponentDTO component)
        {
            playerSetting = component;
            actions = component.Action.GetAction(gameObject);
            IsInitDone = false;
        }

        #region PRIVATE
        private void FixedUpdate()
        {
            if (!IsInitDone && playerSetting.OnCreateAction != null) 
            {
                playerSetting.OnCreateAction.Execute(gameObject);
            }

            if (CheckBindings(playerSetting.Bindings, out var actionType))
            {
                foreach (var action in actions)
                    action.PerformAction(actionType);
            }
            
            if(playerSetting.OnUpdateAction != null)
                playerSetting.OnUpdateAction.Execute(gameObject);
        }

        private bool CheckBindings(List<ActionBindDTO> bindings, out string action)
        {
            foreach (var actionBind in bindings) 
            {
                bool isEqual = true;
                foreach(var binding in actionBind.Binding)
                {
                    if(!Input.GetKeyDown(binding))
                    {
                        isEqual = false;
                        break;
                    }
                }
                if (isEqual) 
                {
                    action = actionBind.ActionType;
                    return true;
                }
            }
            action = "";
            return false;
        }
        #endregion
    }
}
