using UnityEngine;
using Assets.Core.GameEditor.AnimationControllers;
using Assets.Core.GameEditor.Animation;
using UnityEngine.UI;
using Assets.Scripts.GameEditor.ObjectInstancesController;

namespace Assets.Scripts.GameEditor.Controllers
{
    public class AnimationsController : MonoBehaviour, IObjectController
    {
        public IAnimator Animator { get; private set; }
        public bool ShouldLoop {  get; set; }


        private void Update()
        {
            if (Animator == null)
            {
                return;
            }

            if (!Animator.Animating())
                return;

            if (ShouldLoop || !Animator.HasFinished())
                Animator.Animate(Time.deltaTime);
        }

        public void SetCustomAnimation(CustomAnimation animation, bool loop = true, bool playOnAwake = true, float x = 30, float y = 30)
        {
            if (TryGetComponent(out SpriteRenderer spriteRenderer))
            {
               Animator = new SpriteAnimator(spriteRenderer, animation, playOnAwake, x, y);
            }
            else if (TryGetComponent(out Image image))
            {
                Animator = new ImageAnimator(image, animation, playOnAwake);
            }
            else
            {
                var renderer = gameObject.AddComponent<SpriteRenderer>();
                Animator = new SpriteAnimator(renderer, animation, playOnAwake, x, y);
            }
            ShouldLoop = loop;
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

        public bool HasFinishedLoop()
        {
            if (Animator != null)
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

        //public void AddAnimationFrame(string source, float displayTime)
        //{
        //    CheckAnimationConditions();

        //    if (frames == null)
        //        frames = new List<AnimationFrameDTO>();
        //    frames.Add(new AnimationFrameDTO(displayTime, source));
        //}

        //public void SetAnimation(bool shouldLoop)
        //{
        //    SetAnimation(shouldLoop, 0, 0);
        //}

        //public void SetAnimation(bool shouldLoop, float xSize, float ySize)
        //{
        //    if (frames == null)
        //        throw new RuntimeException("You can not set animation before adding at least one frame via \"AddAnimationFrame()\"");

        //    SetSource(new AnimationSourceDTO(frames, "", SourceType.Animation), xSize, ySize, shouldLoop);
        //}

        //public bool IsAnimationComplete()
        //{
        //    return animationController.HasFinishedLoop();
        //}

        //public void PauseAnimation()
        //{
        //    animationController.Pause();
        //}

        //public void ResumeAnimation()
        //{
        //    animationController.Resume();
        //}

        //public void ResetAnimation()
        //{
        //    animationController.ResetClip();
        //}
    }
}
