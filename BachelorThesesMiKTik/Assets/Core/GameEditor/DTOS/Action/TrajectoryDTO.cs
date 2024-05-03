using System.Collections.Generic;
using UnityEngine;

namespace Assets.Core.GameEditor.DTOS.Action
{
    public class TrajectoryDTO
    {
        public List<Vector2> Path { get; set; }
        public Vector2 StartPosition { get; set; }
        public Vector2 MotionDirection { get; set; }
        public Vector2 EndPosition { get; set;}

        public TrajectoryDTO(List<Vector2> path, Vector2 startPosition, Vector2 motionDiretion, Vector2 endPosition)
        {
            Path = path;
            StartPosition = startPosition;
            MotionDirection = motionDiretion;
            EndPosition = endPosition;
        }
    }
}
