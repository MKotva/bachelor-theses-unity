using System;

namespace Assets.Core.GameEditor.DTOS
{
    [Serializable]
    public class AnimationFrameDTO
    {
        public double DisplayTime { get; set; }
        public string URL { get; set; }

        public AnimationFrameDTO(double displayTime, string URL)
        { 
            this.DisplayTime = displayTime;
            this.URL = URL; 
        }
    }
}
