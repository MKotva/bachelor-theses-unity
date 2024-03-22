using Assets.Core.GameEditor.Enums;
using System;

namespace Assets.Core.GameEditor.DTOS
{
    [Serializable]
    public class AssetSourceDTO
    {
        public string Name { get; set; }
        public SourceType Type { get; set; }
        public string URL { get; set; }

        public AssetSourceDTO(SourceType type, string url)
        {
            Type = type;
            URL = url;
        }
    }
}
