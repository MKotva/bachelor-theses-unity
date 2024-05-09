using Assets.Core.GameEditor.Animation;
using UnityEngine;

namespace Assets.Core.GameEditor.AnimationControllers
{
    public class SpriteAnimator : IAnimator
    {
        public bool IsAnimating { get; private set; }
        public bool IsFinished { get; private set; }
        public CustomAnimation Animation { get; private set; }
        public float XScaling { get; private set; }
        public float YScaling { get; private set; }

        public SpriteAnimator(SpriteRenderer renderer, CustomAnimation newAnimation, bool isAnimating, float xSize = 30, float ySize = 30)
        {
            IsAnimating = isAnimating;
            spriteRenderer = renderer;
            Animation = newAnimation;

            XScaling = xSize;
            YScaling = ySize;

            if (newAnimation.Frames.Count != 0)
                SetAnimationFrame(newAnimation.Frames[0]);

            timeSinceLastFrame = 0;
            index = 0;
            Scale(renderer, xSize, ySize);
        }

        /// <summary>
        /// Changes actual animation to given one.
        /// </summary>
        /// <param name="newAnimation"></param>
        public void EditAnimation(CustomAnimation newAnimation)
        {
            if (newAnimation.Frames.Count != 0)
            {
                Animation = newAnimation;
                SetAnimationFrame(newAnimation.Frames[0]);
            }

            timeSinceLastFrame = 0;
            index = 0;
        }

        /// <summary>
        /// Changes actual animation frame to next one, if frame was diplayed for setted time.
        /// </summary>
        /// <param name="timeDelta"></param>
        public void Animate(float timeDelta)
        {
            if (!IsAnimating || !spriteRenderer.isVisible)
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
            IsFinished = false;
            IsAnimating = true;
        }

        /// <summary>
        /// Removes actual animation
        /// </summary>
        public void RemoveAnimation()
        {
            IsAnimating = false;
            spriteRenderer.sprite = null;
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

        private SpriteRenderer spriteRenderer;
        private CustomAnimationFrame actualFrame;

        private float timeSinceLastFrame;
        private int index;

        private void SetAnimationFrame(CustomAnimationFrame newFrame)
        {
            actualFrame = newFrame;
            spriteRenderer.sprite = newFrame.Sprite;
            Scale(spriteRenderer, XScaling, YScaling);
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

        private void Scale(SpriteRenderer spriteRenderer, float xSize, float ySize)
        {
            var rect = spriteRenderer.sprite.rect;

            var xScale = 1 / ( rect.width / xSize );
            var yScale = 1 / ( rect.height / ySize );

            spriteRenderer.transform.localScale = new Vector3(xScale, yScale, 1);
        }
        #endregion
    }
}
