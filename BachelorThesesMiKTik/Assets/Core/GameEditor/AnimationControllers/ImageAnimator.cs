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

        public void Animate(float timeDelta, uint xSize, uint ySize)
        {
            if (!IsAnimating)
                return;

            timeSinceLastFrame += timeDelta;
            if (timeSinceLastFrame >= Animation.Frames[index].DisplayTime)
            {
                SetAnimationFrame(GetNextCustomFrame());
                Scale(imageRenderer, xSize, ySize);
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

        private void Scale(Image imageRenderer, uint xSize, uint ySize)
        {
            var rect = imageRenderer.sprite.rect;

            var xScale = 1 / ( rect.width / xSize );
            var yScale = 1 / ( rect.height / ySize );

            imageRenderer.transform.localScale = new Vector3(xScale, yScale, 1);
        }

        #endregion
    }

}

