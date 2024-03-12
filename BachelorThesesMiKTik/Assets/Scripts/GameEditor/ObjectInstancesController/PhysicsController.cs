using Assets.Core.GameEditor.DTOS.Components;
using UnityEngine;

namespace Assets.Scripts.GameEditor.ObjectInstancesController
{
    public class PhysicsController : MonoBehaviour, IObjectController
    {
        private PhysicsComponentDTO physicsSetting;
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

        public void Initialized(PhysicsComponentDTO physics)
        {
            physicsSetting = physics;
            GravityScale = physicsSetting.Gravity;
            AngularDrag = physicsSetting.AngularDrag;
            Mass = physicsSetting.Mass;
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
            if (!TryGetComponent(out rigid))
            {
                rigid = gameObject.AddComponent<Rigidbody2D>();
                rigid.sleepMode = RigidbodySleepMode2D.StartAsleep;
            }
            Pause();
        }
    }
}
