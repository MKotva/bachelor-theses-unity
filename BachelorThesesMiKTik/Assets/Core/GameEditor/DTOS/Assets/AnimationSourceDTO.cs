using Assets.Core.GameEditor.Enums;
using System;
using System.Collections.Generic;

namespace Assets.Core.GameEditor.DTOS
{
    [Serializable]
    public class AnimationSourceDTO : AssetSourceDTO
    {
        public List<AnimationFrameDTO> AnimationData {get; set;}

        public AnimationSourceDTO(List<AnimationFrameDTO> data, string name) : base(name, "")
        {
            AnimationData = data;
        }
    }
}
