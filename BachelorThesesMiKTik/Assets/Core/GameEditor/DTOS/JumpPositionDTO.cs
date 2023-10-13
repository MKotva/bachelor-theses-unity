using UnityEngine;

namespace Assets.Core.GameEditor.DTOS
{
    public class JumpPositionDTO
    {
        public Vector3 Position;
        public Vector3 MotionDirection;

        public JumpPositionDTO(Vector2 position, Vector2 motionPower) 
        {
            Position = new Vector3(position.x, position.y);
            MotionDirection = new Vector3(motionPower.x, motionPower.y);
        }
    }
}
