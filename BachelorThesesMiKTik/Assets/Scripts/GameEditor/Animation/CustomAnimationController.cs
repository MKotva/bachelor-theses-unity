using UnityEngine;
using Assets.Core.GameEditor.Animation;
using Assets.Core.GameEditor.AnimationControllers;

namespace Assets.Scripts.GameEditor.Controllers
{
    public class CustomAnimationController : MonoBehaviour
    {
        private CustomAnimator animator;
        private SpriteRenderer spriteRenderer;
        private bool shouldLoop;

        private void Start()
        {
            TryGetComponent(out spriteRenderer);
        }

        private void Update()
        {
            if (spriteRenderer == null || animator == null)
            {
                return;
            }

            if(shouldLoop || !animator.HasFinished)
                animator.Animate(Time.deltaTime);
        }

        public void SetCustomAnimation(CustomAnimation animation, bool loop)
        {
            if (animation == null || animation.Frames.Count == 0)
                return; //TODO: Add exception handling.
            animator = new CustomAnimator(spriteRenderer, animation);
            shouldLoop = loop;
        }

        public void PauseAnimation()
        {
           animator.PauseAnimation();
        }

        public void ResumeAnimation()
        {
            animator.ResumeAnimation();
        }

        public void ResetAnimation()
        {
            animator.ResetAnimation();
        }

        public void StopAfterFinishingLoop()
        {
            shouldLoop = false;
        }

        public bool HasFinishedLoop()
        {
            return animator.HasFinished;
        }

        public Sprite GetAnimationPreview()
        {
            return animator.Animation.Frames[0].Sprite;
        }
    }
}
