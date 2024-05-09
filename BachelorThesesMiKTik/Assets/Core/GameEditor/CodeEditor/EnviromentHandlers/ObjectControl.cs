using Assets.Core.GameEditor.AIActions.Movement;
using Assets.Core.GameEditor.Attributes;
using Assets.Core.SimpleCompiler.Exceptions;
using Assets.Scripts.GameEditor.ObjectInstancesController;
using Assets.Scripts.GameEditor.ObjectInstancesController.Components.Entiti;
using UnityEngine;

namespace Assets.Core.GameEditor.CodeEditor.EnviromentObjects
{
    class ObjectControl : EnviromentObject
    {
        private PhysicsController physics;
        private ObjectController controller;
        private GameObject instance;
        private ActionsAgent actionsAgent;


        [CodeEditorAttribute("Stores value, which represents objects HP."
            + "Default value is 100")]
        public float HP
        {
            get
            {
                if (controller == null)
                    return 0;
                return controller.HP;
            }
            set
            {
                if (controller != null)
                    controller.HP = value;
            }
        }

        [CodeEditorAttribute("Stores value, which represents earned score."
            + "Default value is 0")]
        public float Score
        {
            get
            {
                if (controller == null)
                    return 0;
                return controller.Score;
            }
            set
            {
                if (controller != null)
                    controller.Score = value;
            }
        }

        public override bool SetInstance(GameObject instance)
        {
            controller = instance.GetComponent<ObjectController>();
            if (controller == null)
            {
                return false;
            }

            this.instance = instance;

            instance.TryGetComponent(out physics);
            instance.TryGetComponent(out actionsAgent);
            return true;
        }

        [CodeEditorAttribute("Returns value, which represents actual object velocity in horizontal direction.")]
        public float GetXVelocity()
        {
            if (physics == null)
                throw new RuntimeException($"\"Exception in method \\\"GetXVelocity\\\"! Object is missing Physics Component!");

            if (actionsAgent != null)
                if (actionsAgent.PerformingTask != null)
                    return actionsAgent.Velocity.x;

            return physics.GetVelocityX();
        }

        [CodeEditorAttribute("Returns value, which represents actual object velocity in vertical direction.")]
        public float GetYVelocity()
        {
            if (physics == null)
                throw new RuntimeException($"\"Exception in method \\\"GetYVelocity\\\"! Object is missing Physics Component!");

            if (actionsAgent != null)
                if (actionsAgent.PerformingTask != null)
                    return actionsAgent.Velocity.y;

            return physics.GetVelocityY();
        }

        [CodeEditorAttribute("If players velocity is aproximately equal to zero, returns true. Otherwise returns false.")]
        public bool CheckIfIsMoving()
        {
            if (physics == null)
                throw new RuntimeException($"\"Exception in method \\\"CheckIfIsMoving\\\"! Object is missing Physics Component!");

            if (actionsAgent != null)
                if (actionsAgent.PerformingTask != null)
                    if (actionsAgent.Velocity.magnitude > float.Epsilon)
                        return true;


            if (physics.Rigid.velocity.magnitude > float.Epsilon * 2)
                return true;

            return false;
        }

        [CodeEditorAttribute("Checks if object is on some solid object, with collision box. Collision box has to have" +
            "physical on.")]
        public bool CheckIfStaysOnGround()
        {
            return MotionHelper.CheckIfStaysOnGround(instance);
        }

        [CodeEditorAttribute("Destroyes actual object. If the actual animation or audio clip should be finished before" +
            "object's destruction, set the bool values to true.", "(bool finishAnimation, bool finishAudio)")]
        public void Kill(bool finishAnimation, bool finishAudio)
        {
            controller.Kill(finishAnimation, finishAudio);
        }

        [CodeEditorAttribute("Deactivates physics for actual object. Usefull for moving objects like"
           + "elevators.")]
        public void DeactivatePhysics()
        {
            if (physics == null)
                throw new RuntimeException($"\"Exception in method \\\"ActivatePhysics\\\"! Object is missing Physics Component!");
            physics.Deactivate();
        }

        [CodeEditorAttribute("Activates physics for actual object if exists.")]
        public void ActivatePhysics()
        {
            if (physics == null)
                throw new RuntimeException($"\"Exception in method \\\"ActivatePhysics\\\"! Object is missing Physics Component!");
            physics.Activate();
        }

        [CodeEditorAttribute("Adds force to object", "(num xDirection, num yDirection)")]
        public void AddForceInDirection(float x, float y)
        {
            if (physics == null)
                throw new RuntimeException($"\"Exception in method \\\"AddForceInDirection\\\"! Object is missing Physics Component!");
            physics.Rigid.AddForce(new Vector2(x, y));
        }
    }
}
