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

        public void SetCustomAnimation(CustomAnimation animation, bool loop = true, bool playOnAwake = false, float x = 30, float y = 30)
        {
            if (TryGetComponent(out SpriteRenderer spriteRenderer))
            {
               Animator = new SpriteAnimator(spriteRenderer, animation, playOnAwake, x, y);
            }
            else if (TryGetComponent(out Image image))
            {
                Animator = new ImageAnimator(image, animation, playOnAwake);
            }
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
