using Assets.Core.GameEditor.Components;
using Assets.Scripts.GameEditor.ObjectInstancesController;
using Assets.Scripts.GameEditor.ObjectInstancesController.Components.Entiti;

namespace Assets.Scripts.GameEditor.AI
{
    public class AIObjectController : ActionsAgent, IObjectController
    {
        private AIComponent aiSetting;
        private bool wasPlayed;
        private bool isPlaying;

        public void Initialize(AIComponent component)
        {
            aiSetting = component;
            Actions = aiSetting.Action.GetAction(Performer);
        }

        public void Play()
        {
            if (!wasPlayed)
            {
                aiSetting.OnCreateAction.Execute(gameObject);
                wasPlayed = true;
            }
            isPlaying = true;
        }

        public void Pause()
        {
            isPlaying = false;
        }

        public void Enter() {}

        public void Exit()
        {
            isPlaying = false;
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

        private void FixedUpdate()
        {
            if (!isPlaying)
                return;

            if (aiSetting.OnUpdateAction != null)
                aiSetting.OnUpdateAction.Execute(gameObject);

            PerformActions();
        }
        #endregion
    }
}
