using UnityEngine;

namespace Assets.Core.GameEditor.DTOS
{
    public class JumperDTO
    {
        public Vector2 ColliderSize { get; set; }
        public Vector2 GravityAcceleration { get; set; }
        public float Drag { get; set; }
        public float Mass { get; set; }
        public float TimeTick { get; set; }

        public JumperDTO() { }

        public JumperDTO(Vector2 colliderSize, Vector2 gravityAcceleration, float drag, float mass, float timeTick)
        {
            ColliderSize = colliderSize;
            GravityAcceleration = gravityAcceleration;
            Drag = drag;
            Mass = mass;
            TimeTick = timeTick;
        }
    }
}
