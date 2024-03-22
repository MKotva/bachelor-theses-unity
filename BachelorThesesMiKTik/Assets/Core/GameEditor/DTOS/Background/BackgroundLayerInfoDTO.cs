using System;

namespace Assets.Core.GameEditor.DTOS.Background
{
    [Serializable]
    public class BackgroundLayerInfoDTO
    {
        public AssetSourceDTO Source { get; set; }
        public float XSize { get;set; }
        public float YSize { get;set; }
        public int ID { get; set; }

        public BackgroundLayerInfoDTO(AssetSourceDTO source, float xSize, float ySize, int id) 
        {
            Source = source;
            XSize = xSize;
            YSize = ySize;
            ID = id;
        }
    }
}
