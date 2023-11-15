using Assets.Core.GameEditor.Animation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Core.GameEditor.AnimationControllers
{
    public class CustomAnimator
    {
        public bool IsAnimating { get; private set; }

        private SpriteRenderer spriteRenderer;
        private CustomAnimation animation; //TODO: add event driven system. Something like list off events with animations to change the animation.
        private CustomAnimationFrame actualFrame;

        private float timeSinceLastFrame;
        private int index;

        public CustomAnimator(SpriteRenderer renderer, CustomAnimation newAnimation) 
        {
            IsAnimating = true;
            spriteRenderer = renderer;
            animation = newAnimation;
            SetAnimationFrame(newAnimation.Frames[0]);

            timeSinceLastFrame = 0;
            index = 0;
        }

        public void Animate(float timeDelta)
        {
            if (!IsAnimating)
                return;

            timeSinceLastFrame += timeDelta;
            if (timeSinceLastFrame >= animation.Frames[index].DisplayTime)
            {
                SetAnimationFrame(GetNextCustomFrame());
                Scale(spriteRenderer, 1920, 1080);
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
            actualFrame = animation.Frames[0];
            index = 0;
        }

        private void SetAnimationFrame(CustomAnimationFrame newFrame)
        {
            actualFrame = newFrame;
            spriteRenderer.sprite = newFrame.Sprite;
            timeSinceLastFrame = 0;
        }

        private CustomAnimationFrame GetNextCustomFrame()
        {
            index++;
            if (index >= animation.Frames.Count)
            {
                index = 0;
            }

            return animation.Frames[index];
        }

        private void Scale(SpriteRenderer spriteRenderer, uint xSize, uint ySize)
        {
            var width = spriteRenderer.sprite.bounds.size.x;
            var height = spriteRenderer.sprite.bounds.size.y;

            var worldScreenHeight = Camera.main.orthographicSize * 2.0;
            var worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

            spriteRenderer.transform.localScale = new Vector3((float) worldScreenWidth / width, (float) worldScreenHeight / height, 1);
        }
    }
}
