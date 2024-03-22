using Assets.Core.GameEditor.Animation;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Core.GameEditor.AnimationControllers
{
    public class ImageAnimator : IAnimator
    {
        public bool IsAnimating { get; private set; }
        public bool IsFinished { get; private set; }
        public CustomAnimation Animation { get; private set; }

        public ImageAnimator(Image renderer, CustomAnimation newAnimation, bool isAnimating = false)
        {
            this.IsAnimating = isAnimating;
            imageRenderer = renderer;
            Animation = newAnimation;

            if (newAnimation.Frames.Count != 0)
                SetAnimationFrame(newAnimation.Frames[0]);

            timeSinceLastFrame = 0;
            index = 0;
        }

        public void EditAnimation(CustomAnimation newAnimation)
        {
            if (newAnimation.Frames.Count != 0)
                SetAnimationFrame(newAnimation.Frames[0]);

            timeSinceLastFrame = 0;
            index = 0;
        }

        public void Animate(float timeDelta)
        {
            if (!IsAnimating)
                return;

            timeSinceLastFrame += timeDelta;
            if (timeSinceLastFrame >= Animation.Frames[index].DisplayTime)
            {
                SetAnimationFrame(GetNextCustomFrame());
            }
        }

        public void PauseAnimation()
        {
            IsAnimating = false;
        }

        public void ResumeAnimation()
        {
            IsAnimating = true;
        }

        public void ResetAnimation()
        {
            timeSinceLastFrame = 0;
            actualFrame = Animation.Frames[0];
            index = 0;
            IsAnimating = true;
        }

        public void RemoveAnimation()
        {
            IsAnimating = false;
            imageRenderer.sprite = null;
        }

        public void Stop()
        {
            timeSinceLastFrame = 0;
            actualFrame = Animation.Frames[0];
            index = 0;
            IsAnimating = false;
        }

        public bool HasFinished()
        {
            return IsFinished;
        }

        public bool Animating()
        {
            return IsAnimating;
        }

        public CustomAnimation GetAnimation()
        {
            return Animation;
        }

        public float GetXScaling()
        {
            return 1;
        }

        public float GetYScaling()
        {
            return 1;
        }

        #region PRIVATE
        private Image imageRenderer;
        private CustomAnimationFrame actualFrame;

        private float timeSinceLastFrame;
        private int index;

        private void SetAnimationFrame(CustomAnimationFrame newFrame)
        {
            actualFrame = newFrame;
            imageRenderer.sprite = newFrame.Sprite;
            timeSinceLastFrame = 0;
        }

        private CustomAnimationFrame GetNextCustomFrame()
        {
            index++;
            if (index >= Animation.Frames.Count)
            {
                index = 0;
                IsFinished = true;
            }

            return Animation.Frames[index];
        }

        #endregion
    }

}

