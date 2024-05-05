using Assets.Core.GameEditor.DTOS.Assets;
using Assets.Core.GameEditor.DTOS.Background;
using Assets.Core.GameEditor.Enums;
using System;
using System.Collections.Generic;

namespace Assets.Core.GameEditor.DTOS
{
    [Serializable]
    public class BackgroundDTO
    {
        public List<BackgroundReference> LayersSources;
        public SourceReference AudioSource;
        public BackgroundDTO() 
        {
            LayersSources = new List<BackgroundReference>();
            AudioSource = new SourceReference("", SourceType.Sound);
        }

        public BackgroundDTO(List<BackgroundReference> layersInfo, SourceReference audioSource)
        {
            LayersSources = layersInfo;
            AudioSource = audioSource;
        }
    }
}
