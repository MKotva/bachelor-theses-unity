using Assets.Core.GameEditor.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Core.GameEditor.DTOS
{
    public class AnimationSourceDTO : SourceDTO
    {
        public List<AnimationFrameDTO> AnimationData {get; set;}
        public AnimationSourceDTO(SourceType type, List<AnimationFrameDTO> data) : base(type, "")
        {
            AnimationData = data;
        }
    }
}
