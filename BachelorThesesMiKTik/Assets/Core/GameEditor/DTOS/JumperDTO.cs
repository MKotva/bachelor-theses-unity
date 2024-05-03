using UnityEngine;

namespace Assets.Core.GameEditor.DTOS
{
    public class JumperDTO
    {
        public GameObject Performer { get; set; }
        public Vector2 ColliderSize { get; set; }
        public Vector2 GravityAcceleration { get; set; }
        public float Drag { get; set; }
        public float Mass { get; set; }
        public float TimeTick { get; set; }

        public JumperDTO() { }

        public JumperDTO(GameObject performer, Vector2 colliderSize, Vector2 gravityAcceleration, float drag, float mass, float timeTick)
        {
            Performer = performer; 
            ColliderSize = colliderSize;
            GravityAcceleration = gravityAcceleration;
            Drag = drag;
            Mass = mass;
            TimeTick = timeTick;
        }
    }
}
