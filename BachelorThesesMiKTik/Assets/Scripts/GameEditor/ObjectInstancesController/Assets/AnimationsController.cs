﻿using UnityEngine;
using Assets.Core.GameEditor.AnimationControllers;
using Assets.Core.GameEditor.Animation;
using UnityEngine.UI;
using Assets.Scripts.GameEditor.ObjectInstancesController;
using Assets.Scripts.GameEditor.Managers;
using Assets.Core.GameEditor.DTOS.Assets;

namespace Assets.Scripts.GameEditor.Controllers
{
    public class AnimationsController : MonoBehaviour, IObjectController
    {
        public IAnimator Animator { get; private set; }
        public bool ShouldLoop {  get; set; }
        public bool IsManualyPaused { get; set; }
        public SourceReference SourceReference { get; private set; }

        public bool WasCreatedFromCode = false;

        private SourceReference snapShot;

        private void Update()
        {
            if (Animator == null)
            {
                return;
            }

            if (!Animator.Animating())
                return;

            if (!HasFinished())
                Animator.Animate(Time.deltaTime);
        }

        /// <summary>
        /// Sets given animation to created Animatior for this object. Also stores reference to used animation.
        /// </summary>
        /// <param name="animation">Animation to be set.</param>
        /// <param name="source">Reference to animation</param>
        /// <param name="loop">Decides if Animator should play animation in loop.</param>
        /// <param name="playOnAwake">Decides if Animator should play animation right after applying.</param>
        public void SetCustomAnimation(CustomAnimation animation, SourceReference source, bool loop = true, bool playOnAwake = true)
        {
            if (TryGetComponent(out SpriteRenderer spriteRenderer))
            {
               Animator = new SpriteAnimator(spriteRenderer, animation, playOnAwake, source.XSize, source.YSize);
            }
            else if (TryGetComponent(out Image image))
            {
                Animator = new ImageAnimator(image, animation, playOnAwake);
            }
            else
            {
                var renderer = gameObject.AddComponent<SpriteRenderer>();
                Animator = new SpriteAnimator(renderer, animation, playOnAwake, source.XSize, source.YSize);
            }

            ShouldLoop = loop;
            SourceReference = source;
        }

        /// <summary>
        /// Changes actual animation to given one. Reference is same, because animation edit does not
        /// change name.
        /// </summary>
        /// <param name="animation"></param>
        public void EditCutomAnimation(CustomAnimation animation)
        {
            if (Animator == null)
                return;
            Animator.EditAnimation(animation);
        }

        /// <summary>
        /// Removes actual animation and its reference.
        /// </summary>
        public void RemoveAnimation()
        {
            if (Animator != null)
                Animator.RemoveAnimation();
            SourceReference = null;
            Animator = null;
        }

        /// <summary>
        /// Resets actual animation, if exists.
        /// </summary>
        public void ResetClip()
        {
            if (Animator != null)
            {
                IsManualyPaused = false;
                Animator.ResetAnimation();
            }
        }

        /// <summary>
        /// Stops actual animation, after finishing the loop.
        /// </summary>
        public void StopAfterFinishingLoop()
        {
            ShouldLoop = false;
        }

        /// <summary>
        /// Checks if animation is finished. Always False animation is in loop.
        /// </summary>
        /// <returns></returns>
        public bool HasFinished()
        {
            if (Animator != null && !ShouldLoop)
                return Animator.HasFinished();
            return false;
        }
        #region IObjectMethods
        public void Play()
        {
            if (Animator != null && !IsManualyPaused)
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
            {
                IsManualyPaused = false;
                Animator.ResumeAnimation();
            }
        }

        public void Stop()
        {
            if (Animator != null)
            {
                IsManualyPaused = false;
                Animator.Stop();
            }
        }

        public void Enter() 
        {
            if (SourceReference != null)
                snapShot = SourceReference;
        }

        public void Exit()
        {   
            Stop();
            IsManualyPaused = false;
            if(WasCreatedFromCode) 
            {
                RemoveController();
                Animator = null;
                SourceReference = null;
            }
            Restore();
        }
        #endregion

        private void Awake()
        {
            if (TryGetComponent<ObjectController>(out var controller))
            {
                controller.Components.Add(typeof(AnimationsController), this);
            }
        }

        private void OnDestroy()
        {
            RemoveController();
        }

        private void RemoveController()
        {
            var instance = AnimationsManager.Instance;
            if (instance != null && SourceReference != null)
                instance.RemoveActiveController(SourceReference.Name, this);
        }

        private void Restore()
        {
            var instance = AnimationsManager.Instance;
            if (snapShot != null && instance != null)
            {

                if(SourceReference != null)
                    if (snapShot.Name == SourceReference.Name)
                        return;

                RemoveController();
                if(instance.ContainsName(snapShot.Name))
                {
                    instance.SetAnimation(this, snapShot, true, true);
                    Stop();

                    if(TryGetComponent<SpriteRenderer>(out var renderer))
                    {
                        renderer.sprite = instance.GetAnimationPreview(snapShot.Name);
                    }
                }
            }    
        }
    }
}
