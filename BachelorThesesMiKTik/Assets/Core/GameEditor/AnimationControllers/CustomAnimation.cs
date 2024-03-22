using System.Collections.Generic;
using UnityEngine;

namespace Assets.Core.GameEditor.Animation
{
    public class CustomAnimation
    {
        public List<CustomAnimationFrame> Frames { get; private set;}

        public CustomAnimation(List<CustomAnimationFrame> frames) 
        {
            Frames = frames; 
        }
    }
    public class CustomAnimationFrame
    {
        public double DisplayTime { get; set; }
        public Sprite Sprite { get; set; }

        public CustomAnimationFrame(double displayTime, Sprite sprite)
        {
            DisplayTime = displayTime;
            Sprite = sprite;
        }
    }
}
