using UnityEngine;
using Assets.Core.GameEditor.AnimationControllers;
using Assets.Core.GameEditor.Animation;
using UnityEngine.UI;
using Assets.Scripts.GameEditor.ObjectInstancesController;
using Assets.Scripts.GameEditor.Managers;
using Assets.Core.GameEditor.DTOS.Assets;

namespace Assets.Scripts.GameEditor.Controllers
{
    public class AnimationsController : MonoBehaviour, IObjectController
    {
        public IAnimator Animator { get; private set; }
        public bool ShouldLoop {  get; set; }
        public SourceReference SourceReference { get; private set; }


        private void Update()
        {
            if (Animator == null)
            {
                return;
            }

            if (!Animator.Animating())
                return;

            if (!Animator.HasFinished())
                Animator.Animate(Time.deltaTime);
        }

        public void SetCustomAnimation(CustomAnimation animation, SourceReference source, bool loop = true, bool playOnAwake = true)
        {
            if (TryGetComponent(out SpriteRenderer spriteRenderer))
            {
               Animator = new SpriteAnimator(spriteRenderer, animation, playOnAwake, source.XSize, source.YSize);
            }
            else if (TryGetComponent(out Image image))
            {
                Animator = new ImageAnimator(image, animation, playOnAwake);
            }
            else
            {
                var renderer = gameObject.AddComponent<SpriteRenderer>();
                Animator = new SpriteAnimator(renderer, animation, playOnAwake, source.XSize, source.YSize);
            }

            ShouldLoop = loop;
            SourceReference = source;
        }

        public void EditCutomAnimation(CustomAnimation animation)
        {
            if (Animator == null)
                return;
            Animator.EditAnimation(animation);
        }

        public void RemoveAnimation()
        {
            Animator.RemoveAnimation();
        }

        public void Play()
        {
            if (Animator != null)
                Animator.ResetAnimation();
        }

        public void Pause()
        {
            if (Animator != null)
                Animator.PauseAnimation();
        }

        public void Resume()
        {
            if (Animator != null)
                Animator.ResumeAnimation();
        }

        public void Stop()
        {
            if (Animator != null)
                Animator.Stop();
        }

        public void Enter() { }

        public void Exit()
        {
            Animator.Stop();
        }

        public void ResetClip()
        {
            if (Animator != null)
                Animator.ResetAnimation();
        }

        public void StopAfterFinishingLoop()
        {
            ShouldLoop = false;
        }

        public bool CheckIfIsFinished()
        {
            if (Animator != null && !ShouldLoop)
                return Animator.HasFinished();
            return false;
        }

        private void Awake()
        {
            if (TryGetComponent<ObjectController>(out var controller))
            {
                controller.Components.Add(typeof(AnimationsController), this);
            }
        }

        private void OnDestroy()
        {
            var instance = AnimationsManager.Instance;
            if (instance != null && SourceReference != null)
                instance.RemoveActiveController(SourceReference.Name, this);
        }
    }
}
