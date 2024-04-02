using Assets.Core.GameEditor.Components;
using UnityEngine;

namespace Assets.Scripts.GameEditor.ObjectInstancesController
{
    public class PhysicsController : MonoBehaviour, IObjectController
    {
        private PhysicsComponent physicsSetting;
        private Rigidbody2D rigid;

        public float GravityScale
        {
            get
            {
                return rigid.gravityScale;
            }
            set
            {
                rigid.gravityScale = value;
            }
        }

        public float AngularDrag
        {
            get
            {
                return rigid.angularDrag;
            }
            set
            {
                rigid.angularDrag = value;
            }
        }

        public float LinearDrag
        {
            get
            {
                return rigid.drag;
            }
            set
            {
                rigid.drag = value;
            }
        }

        public float Mass
        {
            get
            {
                return rigid.mass;
            }
            set
            {
                rigid.mass = value;
            }
        }

        public bool FreezeZRotation
        {
            get
            {
                if (rigid.constraints == RigidbodyConstraints2D.FreezeRotation ||
                    rigid.constraints == RigidbodyConstraints2D.FreezeAll)
                    return true;
                return false;
            }
            set
            {
                if (value)
                {
                    if (rigid.constraints == RigidbodyConstraints2D.None)
                        rigid.constraints = RigidbodyConstraints2D.FreezeRotation;

                    if (rigid.constraints == RigidbodyConstraints2D.FreezePosition)
                        rigid.constraints = RigidbodyConstraints2D.FreezeAll;
                }
                else
                {
                    if (rigid.constraints == RigidbodyConstraints2D.FreezeRotation)
                        rigid.constraints = RigidbodyConstraints2D.None;

                    if (rigid.constraints == RigidbodyConstraints2D.FreezeAll)
                        rigid.constraints = RigidbodyConstraints2D.FreezePosition;
                }

                rigid.freezeRotation = value;
            }
        }

        public bool FreezeXPosition
        {
            get
            {
                if(rigid.constraints == RigidbodyConstraints2D.FreezePositionX ||
                    rigid.constraints == RigidbodyConstraints2D.FreezePosition ||
                    rigid.constraints == RigidbodyConstraints2D.FreezeAll)
                    return true;
                return false;
            }
            set
            {
                if (value)
                {
                    if (rigid.constraints == RigidbodyConstraints2D.None)
                        rigid.constraints = RigidbodyConstraints2D.FreezePositionX;

                    if (rigid.constraints == RigidbodyConstraints2D.FreezePositionY)
                        rigid.constraints = RigidbodyConstraints2D.FreezePosition;
                }
                else
                {
                    if (rigid.constraints == RigidbodyConstraints2D.FreezePositionX)
                        rigid.constraints = RigidbodyConstraints2D.None;

                    if (rigid.constraints == RigidbodyConstraints2D.FreezePosition)
                        rigid.constraints = RigidbodyConstraints2D.FreezePositionY;

                    if (rigid.constraints == RigidbodyConstraints2D.FreezeAll)
                        rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
                }
            }
        }

        public bool FreezeYPosition
        {
            get
            {
                if (rigid.constraints == RigidbodyConstraints2D.FreezePositionY ||
                    rigid.constraints == RigidbodyConstraints2D.FreezePosition ||
                    rigid.constraints == RigidbodyConstraints2D.FreezeAll)
                    return true;
                return false;
            }
            set
            {
                if (value)
                {
                    if (rigid.constraints == RigidbodyConstraints2D.None)
                        rigid.constraints = RigidbodyConstraints2D.FreezePositionY;

                    if (rigid.constraints == RigidbodyConstraints2D.FreezePositionX)
                        rigid.constraints = RigidbodyConstraints2D.FreezePosition;
                }
                else
                {
                    if (rigid.constraints == RigidbodyConstraints2D.FreezePositionY)
                        rigid.constraints = RigidbodyConstraints2D.None;

                    if (rigid.constraints == RigidbodyConstraints2D.FreezePosition)
                        rigid.constraints = RigidbodyConstraints2D.FreezePositionX;

                    if (rigid.constraints == RigidbodyConstraints2D.FreezeAll)
                        rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
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
            rigid.WakeUp();
            rigid.isKinematic = false;
        }

        public void Pause()
        {
            rigid.Sleep();
            rigid.isKinematic = true;
        }

        public void Enter() {}

        public void Exit()
        {
            rigid.rotation = 0;
            rigid.Sleep();
            rigid.isKinematic = true;

            if (physicsSetting != null)
                Initialize(physicsSetting);
        }

        public float GetVelocityX()
        {
            return rigid.velocity.x;
        }

        public float GetVelocityY()
        {
            return rigid.velocity.y;
        }


        private void Awake()
        {
            if (TryGetComponent<ObjectController>(out var controller))
            {
                controller.Components.Add(typeof(PhysicsController), this);

                if (!TryGetComponent(out rigid))
                {
                    rigid = gameObject.AddComponent<Rigidbody2D>();
                    rigid.sleepMode = RigidbodySleepMode2D.StartAsleep;
                }
                Pause();
            }
        }
    }
}
