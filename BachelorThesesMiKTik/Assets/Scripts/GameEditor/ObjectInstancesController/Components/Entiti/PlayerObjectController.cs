using Assets.Core.GameEditor.Components;
using Assets.Core.GameEditor.DTOS.SourcePanels;
using Assets.Core.SimpleCompiler;
using Assets.Scenes.GameEditor.Core.AIActions;
using Assets.Scripts.GameEditor.ObjectInstancesController;
using Assets.Scripts.GameEditor.ObjectInstancesController.Components.Entiti;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameEditor.Entiti
{
    public class PlayerObjectController : ActionsAgent, IObjectController
    {
        private List<ActionBindDTO> Bindings;
        private List<ActionBase> actions;
        private SimpleCode OnCreate;
        private SimpleCode OnUpdate;


        private delegate void ActionFinishHandler();
        private Dictionary<List<KeyCode>, ActionFinishHandler> actionsFinishers;

        private bool WasPlayed;
        private bool IsPlaying;

        public void Initialize(PlayerComponent component)
        {       
            foreach(ActionBindDTO bind in component.Bindings) 
            {
                if(bind.ActionCode != null)
                    Bindings.Add(new ActionBindDTO(bind.Binding, bind.ActionType, bind.ActionCode));
                else
                    Bindings.Add(new ActionBindDTO(bind.Binding, bind.ActionType));
            }

            actions = component.Actions.GetAction(gameObject);
            
            if(component.OnCreateAction != null) 
            {
                OnCreate = new SimpleCode(component.OnCreateAction);
            }

            if (component.OnUpdateAction != null)
            {
                OnUpdate = new SimpleCode(component.OnUpdateAction);
            }

            actionsFinishers = new Dictionary<List<KeyCode>, ActionFinishHandler>();
            ActionPerformers = component.Actions.GetAction(gameObject);
        }

        #region IObjectMethods
        public void Play()
        {
            if (!WasPlayed)
            {
                if (OnCreate != null)
                    OnCreate.Execute(gameObject);
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
            if (OnCreate != null)
            {
                OnCreate.ResetContext();
            }

            if (OnUpdate != null)
            {
                OnUpdate.ResetContext();
            }

            var gameManager = GameManager.Instance;
            if (gameManager != null)
            {
                gameManager.AddPlayer(gameObject.GetInstanceID(), gameObject);
            }

            originPosition = transform.position;
        }

        public void Exit()
        {
            IsPlaying = false;
            WasPlayed = false;
            ClearActions();
        }
        #endregion

        #region PRIVATE
        protected override void Awake()
        {
            Bindings = new List<ActionBindDTO>();
            base.Awake();
            if (TryGetComponent<ObjectController>(out var controller))
            {
                controller.Components.Add(typeof(PlayerObjectController), this);
            }
        }

        protected override void FixedUpdate()
        {
            if (!IsPlaying)
            {
                return;
            }

            if (ActionsToPerform.Count > 0)
            {
                PerformActions();
                return;
            }

            HandleReleasedKeys();
            if (HandlePressedKeys(Bindings, out var selectedAction))
            {
                if (selectedAction.ActionCode != null)
                    selectedAction.ActionCode.Execute(gameObject);

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

            if (OnUpdate != null)
                OnUpdate.Execute(gameObject);

            CheckFall();

            base.FixedUpdate();
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
