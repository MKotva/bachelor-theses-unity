using Assets.Core.GameEditor.DTOS.Assets;
using System;
using System.Collections.Generic;

namespace Assets.Core.GameEditor.DTOS
{
    [Serializable]
    public class BackgroundDTO
    {
        public List<SourceDTO> LayersSources;
        public BackgroundDTO() 
        {
            LayersSources = new List<SourceDTO>();
        }

        public BackgroundDTO(List<SourceDTO> layersInfo)
        {
            LayersSources = layersInfo;
        }
    }
}
