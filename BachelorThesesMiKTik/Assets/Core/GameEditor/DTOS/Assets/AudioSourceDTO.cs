﻿using Assets.Core.GameEditor.Enums;

namespace Assets.Core.GameEditor.DTOS.Assets
{
    public class AudioSourceDTO : AssetSourceDTO
    {
        public int Priority { get; set; }
        public float StereoPan { get; set; }
        public float Pitch { get; set; }
        public float Volume { get; set; }
        public bool ShouldLoop { get; set; }

        public AudioSourceDTO(string name, string url) : base(SourceType.Sound, url) 
        {
            Name = name;
        }
        public AudioSourceDTO (string name, string url, int priority = 128, float stereoPan = 0, float pitch = 0, float volume = 50, bool shouldLoop = false) : base(SourceType.Sound, url)
        {
            Name = name;
            Priority = priority;
            StereoPan = stereoPan;
            Pitch = pitch;
            Volume = volume;
            ShouldLoop = shouldLoop;
        }
    }
}
