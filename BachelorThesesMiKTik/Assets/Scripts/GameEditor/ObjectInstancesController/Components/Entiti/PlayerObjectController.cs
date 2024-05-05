using Assets.Core.GameEditor.Components;
using Assets.Core.GameEditor.DTOS.SourcePanels;
using Assets.Scenes.GameEditor.Core.AIActions;
using Assets.Scripts.GameEditor.ObjectInstancesController;
using Assets.Scripts.GameEditor.ObjectInstancesController.Components.Entiti;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameEditor.Entiti
{
    public class PlayerObjectController : ActionsAgent, IObjectController
    {
        private PlayerComponent playerSetting;
        private List<ActionBase> actions;

        private delegate void ActionFinishHandler();
        private Dictionary<List<KeyCode>, ActionFinishHandler> actionsFinishers;

        private bool IsInitDone;
        private bool WasPlayed;
        private bool IsPlaying;

        public void Initialize(PlayerComponent component)
        {
            playerSetting = component;
            actions = component.Actions.GetAction(gameObject);
            actionsFinishers = new Dictionary<List<KeyCode>, ActionFinishHandler>();

            ActionPerformers = playerSetting.Actions.GetAction(gameObject);
            IsInitDone = false;
        }

        #region IObjectMethods
        public void Play()
        {
            if (!WasPlayed)
            {
                if (playerSetting.OnCreateAction != null)
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
            if (playerSetting.OnCreateAction != null)
            {
                playerSetting.OnCreateAction.ResetContext();
            }

            if (playerSetting.OnUpdateAction != null)
            {
                playerSetting.OnUpdateAction.ResetContext();
            }

            var gameManager = GameManager.Instance;
            if (gameManager != null)
            {
                gameManager.AddPlayer(gameObject.GetInstanceID(), gameObject);
            }
        }

        public void Exit()
        {
            IsPlaying = false;
            ClearActions();
        }
        #endregion

        #region PRIVATE
        protected override void Awake()
        {
            base.Awake();
            if (TryGetComponent<ObjectController>(out var controller))
            {
                controller.Components.Add(typeof(PlayerObjectController), this);
            }
        }

        private void FixedUpdate()
        {
            if (!IsPlaying)
            {
                return;
            }

            if (!IsInitDone && playerSetting.OnCreateAction != null)
            {
                playerSetting.OnCreateAction.Execute(gameObject);
            }

            if(ActionsToPerform.Count > 0) 
            {
                PerformActions();
                return;
            }

            HandleReleasedKeys();

            if (HandlePressedKeys(playerSetting.Bindings, out var selectedAction))
            {
                if (selectedAction.ActionCode != null)
                    selectedAction.ActionCode.Execute(gameObject);
                else
                {
                    foreach (var action in actions)
                    {
                        if (action.ContainsActionCode(selectedAction.ActionType))
                        {
                            action.PerformAction(selectedAction.ActionType);
                            if (!actionsFinishers.ContainsKey(selectedAction.Binding))
                                actionsFinishers.Add(selectedAction.Binding, action.FinishAction);
                        }
                    }
                }
            }

            if (playerSetting.OnUpdateAction != null)
                playerSetting.OnUpdateAction.Execute(gameObject);

            CheckFall();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bindings"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        private bool HandlePressedKeys(List<ActionBindDTO> bindings, out ActionBindDTO action)
        {
            ActionBindDTO maxBinding = null;
            foreach (var actionBind in bindings)
            {
                bool isEqual = true;
                foreach (var binding in actionBind.Binding)
                {
                    if (!Input.GetKey(binding))
                    {
                        isEqual = false;
                        break;
                    }
                }
                if (isEqual)
                {
                    if (maxBinding == null)
                        maxBinding = actionBind;
                    else if (maxBinding.Binding.Count < actionBind.Binding.Count)
                        maxBinding = actionBind;
                }
            }

            if (maxBinding == null)
            {
                action = null;
                return false;
            }
            action = maxBinding;
            return true;
        }

        private void HandleReleasedKeys()
        {
            var executed = new List<List<KeyCode>>();
            foreach (var binding in actionsFinishers.Keys)
            {
                bool isEqual = true;
                foreach (var key in binding)
                {
                    if (Input.GetKey(key))
                    {
                        isEqual = false;
                        break;
                    }
                }

                if (isEqual)
                {
                    actionsFinishers[binding].Invoke();
                    executed.Add(binding);
                }
            }

            foreach (var key in executed)
                actionsFinishers.Remove(key);
        }

        private void CheckFall()
        {
            var gameManager = GameManager.Instance;
            if (gameManager != null)
            {
                if (gameObject.transform.position.y < gameManager.LowestYPoint - 10)
                {
                    gameManager.RemovePlayer(gameObject.GetInstanceID());
                    gameObject.SetActive(false);
                }
            }
        }
        #endregion
    }
}
