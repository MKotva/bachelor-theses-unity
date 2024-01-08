using Assets.Core.GameEditor.DTOS.Background;
using System;
using System.Collections.Generic;

namespace Assets.Core.GameEditor.DTOS
{
    [Serializable]
    public class BackgroundDTO
    {
        public List<BackgroundLayerInfoDTO> LayersSources;
        public BackgroundDTO() 
        {
            LayersSources = new List<BackgroundLayerInfoDTO>();
        }

        public BackgroundDTO(List<BackgroundLayerInfoDTO> layersInfo)
        {
            LayersSources = layersInfo;
        }
    }
}
