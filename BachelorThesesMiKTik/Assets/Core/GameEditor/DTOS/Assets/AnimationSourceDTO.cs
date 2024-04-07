using Assets.Core.GameEditor.Enums;
using System;
using System.Collections.Generic;

namespace Assets.Core.GameEditor.DTOS
{
    [Serializable]
    public class AnimationSourceDTO : AssetSourceDTO
    {
        public List<AnimationFrameDTO> AnimationData {get; set;}
        public bool Loop { get; set; }

        public bool OnAwake { get; set; }
        public AnimationSourceDTO(List<AnimationFrameDTO> data, string name, bool loop = true, bool onAwake = true) : base(name, "")
        {
            AnimationData = data;
            Loop = loop;
            OnAwake = onAwake;
        }
    }
}
