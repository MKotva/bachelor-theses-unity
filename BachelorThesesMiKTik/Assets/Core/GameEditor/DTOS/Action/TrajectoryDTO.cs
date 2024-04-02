using System.Collections.Generic;
using UnityEngine;

namespace Assets.Core.GameEditor.DTOS.Action
{
    public class TrajectoryDTO
    {
        public List<Vector3> Path { get; set; }
        public Vector3 StartPosition { get; set; }
        public Vector3 MotionDirection { get; set; }
        public Vector3 EndPosition { get; set;}

        public TrajectoryDTO(List<Vector3> path, Vector3 startPosition, Vector3 motionDiretion, Vector3 endPosition)
        {
            Path = path;
            StartPosition = startPosition;
            MotionDirection = motionDiretion;
            EndPosition = endPosition;
        }
    }
}
