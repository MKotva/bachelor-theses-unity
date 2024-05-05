using Assets.Core.GameEditor.Attributes;
using Assets.Core.GameEditor.CodeEditor.EnviromentObjects;
using Assets.Core.GameEditor.DTOS.Assets;
using Assets.Core.GameEditor.Enums;
using Assets.Core.SimpleCompiler.Exceptions;
using Assets.Scripts.GameEditor.Controllers;
using Assets.Scripts.GameEditor.Managers;
using Assets.Scripts.GameEditor.ObjectInstancesController;
using UnityEngine;

namespace Assets.Core.GameEditor.CodeEditor.EnviromentHandlers
{
    public class ObjectVisual : EnviromentObject
    {
        AnimationsController animationController;
        AnimationsManager animationsManager;

        SpriteController spriteController;
        SpriteManager spriteManager;
        public override bool SetInstance(GameObject instance)
        {
            return GetAnimatiosReference(instance) && GetImageReference(instance);
        }

        public bool GetAnimatiosReference(GameObject instance)
        {
            if (!instance.TryGetComponent(out animationController))
            {
                animationController = instance.AddComponent<AnimationsController>();
            }

            animationsManager = AnimationsManager.Instance;
            if (animationsManager == null)
                return false;

            return true;
        }

        public bool GetImageReference(GameObject instance)
        {
            if (!instance.TryGetComponent(out spriteController))
            {
                spriteController = instance.AddComponent<SpriteController>();
            }

            spriteManager = SpriteManager.Instance;
            if (spriteManager == null)
                return false;

            return true;
        }

        #region AnimationMethods

        [CodeEditorAttribute("If animation has shown all images returs true. WARNING: If you set animation for loop, this " +
            "method will always return false.", "(string nameOfAnimation, bool shouldAnimationLoop, bool playAfterSet)")]
        public bool CheckIfIsFinished()
        {
            return animationController.HasFinished();
        }

        [CodeEditorAttribute("Finds created animation by given name and sets it for this object. If there was previous " +
            "animation or image of certain scaling, this scaling will be used for this animation. Otherwise default scaling " +
            "30x30 will be used.", "(string nameOfAnimation, bool shouldAnimationLoop, bool playAfterSet)")]
        public void SetAnimation(string name, bool shouldLoop, bool playAfterSet)
        {
            if (animationController.SourceReference != null)
            {
                var reference = animationController.SourceReference;
                SetAnimation(name, shouldLoop, playAfterSet, reference.XSize, reference.YSize);
                return;
            }
            else if (animationController.gameObject.TryGetComponent(out SpriteController spriteController))
            {
                if (spriteController.SourceReference != null)
                {
                    var reference = spriteController.SourceReference;
                    SetAnimation(name, shouldLoop, playAfterSet, reference.XSize, reference.YSize);
                    return;
                }
            }
            SetAnimation(name, shouldLoop, playAfterSet, 32, 32);
        }

        [CodeEditorAttribute("Finds created animation by given name and sets it for this object.",
            "(string nameOfAnimation, bool shouldAnimationLoop, bool playAfterSet, float xScaling, float yScaling)")]
        public void SetAnimation(string name, bool shouldLoop, bool playAfterSet, float x, float y)
        {
            if (!animationsManager.ContainsName(name))
                throw new RuntimeException($"\"Exception in method \\\"SetAnimation\\\"!  There is no animation with name: {name}");

            var animationSource = new SourceReference(name, Enums.SourceType.Animation, x, y);
            animationsManager.SetAnimation(animationController, animationSource, shouldLoop, playAfterSet);
        }

        [CodeEditorAttribute("Plays setted animation if it is not already running, otherwise nothing happens.")]
        public void PlayAnimation()
        {
            animationController.Play();
        }

        [CodeEditorAttribute("Resumes paused animation on this object. If animation is not paused" +
            "nothing happens.")]
        public void ResumeAnimation()
        {
            animationController.Resume();
        }

        [CodeEditorAttribute("Pauses the actual running animation.")]
        public void PauseAnimation()
        {
            animationController.Pause();
        }


        [CodeEditorAttribute("Stops the actual running animation and sets the first frame of animation as actual image.")]
        public void StopAnimation()
        {
            animationController.Stop();
        }

        [CodeEditorAttribute("Breakes the animation loop after finishing the cycle.")]
        public void StopLooping()
        {
            animationController.StopAfterFinishingLoop();
        }

        [CodeEditorAttribute("Restarts actual running animation.")]
        public void RestartAnimation()
        {
            animationController.ResetClip();
        }

        [CodeEditorAttribute("Removes actual animation from object and sets image to empty.")]
        public void RemoveAnimation()
        {
            animationController.RemoveAnimation();
        }

        #endregion

        #region SpriteMethods

        [CodeEditorAttribute("Finds created image by given name and sets it for this object. If there was previous " +
            "animation or image of certain scaling, this scaling will be used for this image. Otherwise default scaling " +
            "30x30 will be used.", "(string nameOfImage)")]
        public void SetImage(string name)
        {
            if (animationController.SourceReference != null)
            {
                var reference = animationController.SourceReference;
                SetImage(name, reference.XSize, reference.YSize);
                return;
            }
            else if (animationController.gameObject.TryGetComponent(out SpriteController spriteController))
            {
                if (spriteController.SourceReference != null)
                {
                    var reference = spriteController.SourceReference;
                    SetImage(name, reference.XSize, reference.YSize);
                    return;
                }
            }
            SetImage(name, 32, 32);
        }

        [CodeEditorAttribute("Finds created image by given name and sets it for this object.",
           "(string nameOfImage, float xScaling, float yScaling)")]
        public void SetImage(string name, float x, float y)
        {
            if (!spriteManager.ContainsName(name))
            {
                throw new RuntimeException($"\"Exception in method \\\"SetImage\\\"! There is no image with name: {name}");
            }

            var sourceReference = spriteController.SourceReference;
            if (sourceReference != null)
            {
                if (sourceReference.Name != "")
                {
                    spriteManager.RemoveActiveController(sourceReference.Name, spriteController);
                }
            }
            var animationSource = new SourceReference(name, SourceType.Image, x, y);
            spriteManager.SetSprite(spriteController, animationSource);
        }

        [CodeEditorAttribute("Deletes actual image of this object. If there is no image present, nothing happens.")]
        public void DeleteImage()
        {
            spriteController.DeleteSprite();
        }

        [CodeEditorAttribute("Sets color of this object. This includes all actual and future animations and images.")]
        public void SetColor(float r, float g, float b)
        {
            spriteController.ChangeColor(r, g, b);
        }
        #endregion

        [CodeEditorAttribute("Flips actual orientation of image by vertical axis")]
        public void FlipYVisual()
        {
            if (spriteController == null)
                return;
            spriteController.spriteRendered.flipX = !spriteController.spriteRendered.flipX;
        }

        public void RotateVisual(float angle)
        {
        }
    }
}
