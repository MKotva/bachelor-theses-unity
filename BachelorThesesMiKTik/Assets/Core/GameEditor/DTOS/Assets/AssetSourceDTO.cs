using System;

namespace Assets.Core.GameEditor.DTOS
{
    [Serializable]
    public class AssetSourceDTO
    {
        public string Name { get; set; }
        public string URL { get; set; }

        public AssetSourceDTO(string name, string url)
        {
           Name = name;
           URL = url;
        }
    }
}
