using UnityEngine;

namespace Assets.Core.GameEditor.DTOS
{
    public class JumperDTO
    {
        public Vector2 ColliderSize { get; set; }
        public Vector2 GravityAcceleration { get; set; }
        public float Drag { get; set; }

        public JumperDTO() { }

        public JumperDTO(Vector2 colliderSize, Vector2 gravityAcceleration, float drag)
        {
            ColliderSize = colliderSize;
            GravityAcceleration = gravityAcceleration;
            Drag = drag;
        }
    }
}
