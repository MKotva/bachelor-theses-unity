using Assets.Core.GameEditor.Enums;
using System;
using System.Collections.Generic;

namespace Assets.Core.GameEditor.DTOS
{
    [Serializable]
    public class AnimationSourceDTO : SourceDTO
    {
        public List<AnimationFrameDTO> AnimationData {get; set;}
        public string Name { get; set; }

        public bool Loop { get; set; }

        public bool OnAwake { get; set; }
        public AnimationSourceDTO(List<AnimationFrameDTO> data, string name, SourceType type, bool loop = true, bool onAwake = true) : base(type, "")
        {
            AnimationData = data;
            Name = name;
            Loop = loop;
            OnAwake = onAwake;
        }
    }
}
