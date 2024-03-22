using Assets.Core.GameEditor.Components;
using Assets.Core.GameEditor.DTOS.SourcePanels;
using Assets.Scenes.GameEditor.Core.AIActions;
using Assets.Scripts.GameEditor.ObjectInstancesController;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameEditor.Entiti
{
    public class PlayerObjectController : MonoBehaviour, IObjectController
    {
        private PlayerComponent playerSetting;
        private List<AIActionBase> actions;
        private bool IsInitDone;
        
        private bool WasPlayed;
        private bool IsPlaying;

        public void Initialize(PlayerComponent component)
        {
            playerSetting = component;
            actions = component.Action.GetAction(gameObject);
            IsInitDone = false;
        }

        public void Play() 
        { 
            if(!WasPlayed) 
            {
                if(playerSetting.OnCreateAction != null) 
                    playerSetting.OnCreateAction.Execute(gameObject);
                WasPlayed = true;
            }
            IsPlaying = true;
        }

        public void Pause()
        {
            IsPlaying = false;
        }

        public void Enter()
        {

        }

        public void Exit()
        {
            IsPlaying = false;
        }

        #region PRIVATE
        private void Awake()
        {
            if (TryGetComponent<ObjectController>(out var controller))
            {
                controller.Components.Add(typeof(PlayerObjectController), this);
            }
        }

        private void FixedUpdate()
        {
            if(!IsPlaying) 
            {
                return;
            }

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
