using UnityEngine;
using Assets.Core.GameEditor.AnimationControllers;

namespace Assets.Scripts.GameEditor.Controllers
{
    public class AnimationsController : MonoBehaviour
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

        public void SetCustomAnimation(IAnimator animator, bool loop)
        {
            Animator = animator;
            ShouldLoop = loop;
        }

        public void Play()
        {
            Animator.ResetAnimation();
        }

        public void Pause()
        {
            Animator.PauseAnimation();
        }

        public void Resume()
        {
            Animator.ResumeAnimation();
        }

        public void ResetClip()
        {
            Animator.ResetAnimation();
        }

        public void StopAfterFinishingLoop()
        {
            ShouldLoop = false;
        }

        public bool HasFinishedLoop()
        {
            return Animator.HasFinished();
        }

        public Sprite GetAnimationPreview()
        {
            return Animator.GetAnimation().Frames[0].Sprite;
        }
    }
}
