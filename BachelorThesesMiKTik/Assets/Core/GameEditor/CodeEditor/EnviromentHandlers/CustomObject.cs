using Assets.Core.GameEditor.DTOS;
using Assets.Core.GameEditor.Enums;
using Assets.Core.SimpleCompiler.Exceptions;
using Assets.Scripts.GameEditor.GameObjects.Elements;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Core.GameEditor.CodeEditor.EnviromentObjects
{
    class CustomObject : EnviromentObject
    {
        public CustomObjectController CustomController { get; set; }
        private List<AnimationFrameDTO> frames;

        public int HP { get; set; }
        public int Score { get; set; }

        public float MaxSpeed { get; set; } = 5f;

        public override void SetInstance(GameObject instance) { }

        public float GravityScale
        {
            get
            {
                CheckRigidBodyCondition("Gravity scale");
                return CustomController.Rigidbody.gravityScale;
            }
            set
            {
                CheckRigidBodyCondition("Gravity scale");
                CustomController.Rigidbody.gravityScale = value;
            }
        }

        public float AngularDrag
        {
            get
            {
                CheckRigidBodyCondition("Angular drag");
                return CustomController.Rigidbody.angularDrag;
            }
            set
            {
                CheckRigidBodyCondition("Angular drag");
                CustomController.Rigidbody.angularDrag = value;
            }
        }

        public float Mass
        {
            get
            {
                CheckRigidBodyCondition("Mass");
                return CustomController.Rigidbody.mass;
            }
            set
            {
                CheckRigidBodyCondition("Mass");
                CustomController.Rigidbody.mass = value;
            }
        }

        public float GetVelocityX()
        {
            CheckRigidBodyCondition("Velocity");
            return CustomController.Rigidbody.velocity.x;
        }

        public float GetVelocityY()
        {
            CheckRigidBodyCondition("Velocity");
            return CustomController.Rigidbody.velocity.y;
        }

        #region Sprite
        public void ChangeColor(float r , float g, float b)
        {
            CheckImageConditions();
            CustomController.SpriteRenderer.color = new Color(r, g, b);
        }

        public void SetImage(string source)
        {
            SetImage(source, 0, 0);
        }

        public void SetImage(string source, float xSize, float ySize)
        {
            CheckImageConditions();

            var imageSource = new SourceDTO(SourceType.Image, source);
            CustomController.SetSource(imageSource, xSize, ySize);
        }
        #endregion

        #region Animation
        public void AddAnimationFrame(string source, float displayTime)
        {
            CheckAnimationConditions();

            if (frames == null)
                frames = new List<AnimationFrameDTO>();
            frames.Add(new AnimationFrameDTO(displayTime, source));
        }

        public void SetAnimation(bool shouldLoop) 
        {
            SetAnimation(shouldLoop, 0, 0);
        }

        public void SetAnimation(bool shouldLoop, float xSize, float ySize)
        {
            if (frames == null)
                throw new RuntimeException("You can not set animation before adding at least one frame via \"AddAnimationFrame()\"");

            CustomController.SetSource(new AnimationSourceDTO(SourceType.Animation, frames), xSize, ySize, shouldLoop);
        }

        public bool IsAnimationComplete()
        {
            CheckAnimationConditions();
            return CustomController.AnimationController.HasFinishedLoop();
        }

        public void PauseAnimation()
        {
            CheckAnimationConditions();
            CustomController.AnimationController.PauseAnimation();
        }

        public void ResumeAnimation()
        {
            CheckAnimationConditions();
            CustomController.AnimationController.ResumeAnimation();
        }

        public void ResetAnimation()
        {
            CheckAnimationConditions();
            CustomController.AnimationController.ResetAnimation();
        }
        #endregion

        #region PRIVATE
        private void CheckAnimationConditions()
        {
            CheckImageConditions();
            if (CustomController.AnimationController == null)
                throw new RuntimeException("Object does not contain animator component");
        }

        private void CheckImageConditions()
        {
            if (CustomController.SpriteRenderer == null)
                throw new RuntimeException("Object does not contain image component");
        }

        private void CheckRigidBodyCondition(string propName)
        {
            if (CustomController.Rigidbody == null)
                throw new RuntimeException($"Unable to acces property {propName}! Object does not contains RigidBody component!");
        }

        #endregion
    }
}
