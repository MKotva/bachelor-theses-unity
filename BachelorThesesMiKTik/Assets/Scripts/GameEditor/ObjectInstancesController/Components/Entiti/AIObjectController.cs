using Assets.Core.GameEditor.Components;
using Assets.Core.SimpleCompiler;
using Assets.Scripts.GameEditor.ObjectInstancesController;
using Assets.Scripts.GameEditor.ObjectInstancesController.Components.Entiti;
using UnityEngine;

namespace Assets.Scripts.GameEditor.AI
{
    public class AIObjectController : ActionsAgent, IObjectController
    {
        private SimpleCode OnCreate;
        private SimpleCode OnUpdate;
        private bool isPlaying;

        public void Initialize(AIComponent component)
        {
            if(component.OnCreateAction != null) 
            {
                OnCreate = new SimpleCode(component.OnCreateAction);
            }

            if (component.OnUpdateAction != null)
            {
                OnUpdate = new SimpleCode(component.OnUpdateAction);
            }

            ActionPerformers = component.Action.GetAction(Performer);
        }

        public void Play()
        {
            isPlaying = true;
            isPerforming = true;
        }

        public void Pause()
        {
            isPlaying = false;
            isPerforming = false;
        }

        public void Enter() 
        {
            if (OnCreate != null)
            {
                OnCreate.ResetContext();
                OnCreate.Execute(gameObject);
            }

            if (OnUpdate != null)
            {
                OnUpdate.ResetContext();
            }

            originPosition = transform.position;
        }

        public void Exit()
        {
            isPlaying = false;
            isPerforming = false;
            ClearActions();
        }

        #region PRIVATE
        protected override void Awake()
        {
            base.Awake();         
            if (TryGetComponent<ObjectController>(out var controller))
            {
                controller.Components.Add(typeof(AIObjectController), this);
            }
        }

        protected override void FixedUpdate()
        {
            if (!isPlaying)
                return;

            if (OnUpdate != null)
                OnUpdate.Execute(gameObject);

            PerformActions();
            base.FixedUpdate();
        }
        #endregion
    }
}
