using Assets.Core.GameEditor.Enums;
using System;
using System.Collections.Generic;

namespace Assets.Core.GameEditor.DTOS
{
    [Serializable]
    public class AnimationSourceDTO : SourceDTO
    {
        public List<AnimationFrameDTO> AnimationData {get; set;}
        public AnimationSourceDTO(SourceType type, List<AnimationFrameDTO> data) : base(type, "")
        {
            AnimationData = data;
        }
    }
}
