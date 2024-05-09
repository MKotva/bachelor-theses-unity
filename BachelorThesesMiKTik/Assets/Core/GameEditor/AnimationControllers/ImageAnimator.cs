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

        /// <summary>
        /// Changes actual animation to given one.
        /// </summary>
        /// <param name="newAnimation"></param>
        public void EditAnimation(CustomAnimation newAnimation)
        {
            if (newAnimation.Frames.Count != 0)
                SetAnimationFrame(newAnimation.Frames[0]);

            timeSinceLastFrame = 0;
            index = 0;
        }

        /// <summary>
        /// Changes actual animation frame to next one, if frame was diplayed for setted time.
        /// </summary>
        /// <param name="timeDelta"></param>
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

        /// <summary>
        /// Pauses actual animation
        /// </summary>
        public void PauseAnimation()
        {
            IsAnimating = false;
        }

        /// <summary>
        /// Resumes actual animation
        /// </summary>
        public void ResumeAnimation()
        {
            IsAnimating = true;
        }

        /// <summary>
        /// Reset actual animation to initial state
        /// </summary>
        public void ResetAnimation()
        {
            timeSinceLastFrame = 0;
            actualFrame = Animation.Frames[0];
            index = 0;
            IsAnimating = true;
        }

        /// <summary>
        /// Removes actual animation
        /// </summary>
        public void RemoveAnimation()
        {
            IsAnimating = false;
            imageRenderer.sprite = null;
        }

        /// <summary>
        /// Reset actual animation to initial state and stops animation.
        /// </summary>
        public void Stop()
        {
            timeSinceLastFrame = 0;
            actualFrame = Animation.Frames[0];
            index = 0;
            IsAnimating = false;
        }

        /// <summary>
        /// Checks if animation is finished.
        /// </summary>
        /// <returns></returns>
        public bool HasFinished()
        {
            return IsFinished;
        }

        //Checks if is animating.
        public bool Animating()
        {
            return IsAnimating;
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

