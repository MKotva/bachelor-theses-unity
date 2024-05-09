using Assets.Core.GameEditor.Components;
using UnityEngine;

namespace Assets.Scripts.GameEditor.ObjectInstancesController
{
    public class PhysicsController : MonoBehaviour, IObjectController
    {
        public Rigidbody2D Rigid { get; private set; }
        private PhysicsComponent physicsSetting;
        private bool isManualyDeactivated;
        public float GravityScale
        {
            get
            {
                return Rigid.gravityScale;
            }
            set
            {
                Rigid.gravityScale = value;
            }
        }

        public float AngularDrag
        {
            get
            {
                return Rigid.angularDrag;
            }
            set
            {
                Rigid.angularDrag = value;
            }
        }

        public float LinearDrag
        {
            get
            {
                return Rigid.drag;
            }
            set
            {
                Rigid.drag = value;
            }
        }

        public float Mass
        {
            get
            {
                return Rigid.mass;
            }
            set
            {
                Rigid.mass = value;
            }
        }

        public bool FreezeZRotation
        {
            get
            {
                if (Rigid.constraints == RigidbodyConstraints2D.FreezeRotation ||
                    Rigid.constraints == RigidbodyConstraints2D.FreezeAll)
                    return true;
                return false;
            }
            set
            {
                if (value)
                {
                    if (Rigid.constraints == RigidbodyConstraints2D.None)
                        Rigid.constraints = RigidbodyConstraints2D.FreezeRotation;

                    if (Rigid.constraints == RigidbodyConstraints2D.FreezePosition)
                        Rigid.constraints = RigidbodyConstraints2D.FreezeAll;
                }
                else
                {
                    if (Rigid.constraints == RigidbodyConstraints2D.FreezeRotation)
                        Rigid.constraints = RigidbodyConstraints2D.None;

                    if (Rigid.constraints == RigidbodyConstraints2D.FreezeAll)
                        Rigid.constraints = RigidbodyConstraints2D.FreezePosition;
                }

                Rigid.freezeRotation = value;
            }
        }

        public bool FreezeXPosition
        {
            get
            {
                if(Rigid.constraints == RigidbodyConstraints2D.FreezePositionX ||
                    Rigid.constraints == RigidbodyConstraints2D.FreezePosition ||
                    Rigid.constraints == RigidbodyConstraints2D.FreezeAll)
                    return true;
                return false;
            }
            set
            {
                if (value)
                {
                    if (Rigid.constraints == RigidbodyConstraints2D.None)
                        Rigid.constraints = RigidbodyConstraints2D.FreezePositionX;

                    if (Rigid.constraints == RigidbodyConstraints2D.FreezePositionY)
                        Rigid.constraints = RigidbodyConstraints2D.FreezePosition;
                }
                else
                {
                    if (Rigid.constraints == RigidbodyConstraints2D.FreezePositionX)
                        Rigid.constraints = RigidbodyConstraints2D.None;

                    if (Rigid.constraints == RigidbodyConstraints2D.FreezePosition)
                        Rigid.constraints = RigidbodyConstraints2D.FreezePositionY;

                    if (Rigid.constraints == RigidbodyConstraints2D.FreezeAll)
                        Rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
                }
            }
        }

        public bool FreezeYPosition
        {
            get
            {
                if (Rigid.constraints == RigidbodyConstraints2D.FreezePositionY ||
                    Rigid.constraints == RigidbodyConstraints2D.FreezePosition ||
                    Rigid.constraints == RigidbodyConstraints2D.FreezeAll)
                    return true;
                return false;
            }
            set
            {
                if (value)
                {
                    if (Rigid.constraints == RigidbodyConstraints2D.None)
                        Rigid.constraints = RigidbodyConstraints2D.FreezePositionY;

                    if (Rigid.constraints == RigidbodyConstraints2D.FreezePositionX)
                        Rigid.constraints = RigidbodyConstraints2D.FreezePosition;
                }
                else
                {
                    if (Rigid.constraints == RigidbodyConstraints2D.FreezePositionY)
                        Rigid.constraints = RigidbodyConstraints2D.None;

                    if (Rigid.constraints == RigidbodyConstraints2D.FreezePosition)
                        Rigid.constraints = RigidbodyConstraints2D.FreezePositionX;

                    if (Rigid.constraints == RigidbodyConstraints2D.FreezeAll)
                        Rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
                }
            }
        }

        public void Initialize(PhysicsComponent physics)
        {
            physicsSetting = physics;
            GravityScale = physicsSetting.Gravity;
            AngularDrag = physicsSetting.AngularDrag;
            LinearDrag = physicsSetting.LinearDrag;
            Mass = physicsSetting.Mass;
            FreezeZRotation = physicsSetting.IsZRotationFreeze;
            FreezeXPosition = physicsSetting.IsXPositionFreeze;
            FreezeYPosition = physicsSetting.IsYPositionFreeze;
        }

        public void Play()
        {
            if (!isManualyDeactivated)
            {
                Rigid.WakeUp();
                Rigid.isKinematic = false;
            }
        }

        public void Pause()
        {
            Rigid.Sleep();
            Rigid.isKinematic = true;
        }

        public void Enter() {}

        public void Exit()
        {
            Rigid.Sleep();
            Rigid.isKinematic = true;

            if (physicsSetting != null)
                Initialize(physicsSetting);

            isManualyDeactivated = false;
        }

        public float GetVelocityX()
        {
            return Rigid.velocity.x;
        }

        public float GetVelocityY()
        {
            return Rigid.velocity.y;
        }


        public void Activate()
        {
            isManualyDeactivated = false;
            TurnOn();
        }

        public void Deactivate()
        {
            isManualyDeactivated = true;
            TurnOff();
        }


        private void Awake()
        {
            if (TryGetComponent<ObjectController>(out var controller))
            {
                controller.Components.Add(typeof(PhysicsController), this);

                Rigidbody2D rigid;
                if (!TryGetComponent(out rigid))
                {
                    Rigid = gameObject.AddComponent<Rigidbody2D>();
                    Rigid.sleepMode = RigidbodySleepMode2D.StartAsleep;
                }
                else
                {
                    Rigid = rigid;
                }

                Pause();
            }
        }

        private void TurnOff()
        {
            Rigid.Sleep();
            Rigid.isKinematic = true;
        }

        private void TurnOn()
        {
            Rigid.WakeUp();
            Rigid.isKinematic = false;
        }
    }
}
