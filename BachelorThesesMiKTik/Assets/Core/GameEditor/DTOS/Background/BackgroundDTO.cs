using Assets.Core.GameEditor.DTOS.Assets;
using Assets.Core.GameEditor.Enums;
using System;
using System.Collections.Generic;

namespace Assets.Core.GameEditor.DTOS
{
    [Serializable]
    public class BackgroundDTO
    {
        public List<SourceReference> LayersSources;
        public SourceReference AudioSource;
        public BackgroundDTO() 
        {
            LayersSources = new List<SourceReference>();
            AudioSource = new SourceReference("", SourceType.Sound);
        }

        public BackgroundDTO(List<SourceReference> layersInfo, SourceReference audioSource)
        {
            LayersSources = layersInfo;
            AudioSource = audioSource;
        }
    }
}
