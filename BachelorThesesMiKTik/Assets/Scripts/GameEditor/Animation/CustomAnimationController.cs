using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Assets.Core.GameEditor.Animation;
using UnityEngine.Rendering;
using Assets.Core.GameEditor.AnimationControllers;

namespace Assets.Scripts.GameEditor.Controllers
{
    public class CustomAnimationController : MonoBehaviour
    {
        private CustomAnimator animator;
        private SpriteRenderer spriteRenderer;

        private void Start()
        {
            TryGetComponent(out spriteRenderer);
        }

        private void FixedUpdate()
        {
            if (spriteRenderer == null || animator == null)
            {
                return;
            }

            animator.Animate(Time.fixedDeltaTime);
        }

        public void SetCustomAnimation(CustomAnimation animation)
        {
            if (animation == null || animation.Frames.Count == 0)
                return; //TODO: Add exception handling.
            animator = new CustomAnimator(spriteRenderer, animation);
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

        public Sprite GetAnimationPreview()
        {
            return animator.Animation.Frames[0].Sprite;
        }
    }
}
