using Assets.Core.GameEditor.Enums;
using System;

namespace Assets.Core.GameEditor.DTOS
{
    [Serializable]
    public class SourceDTO
    {
        public SourceType Type { get; set; }
        public string URL { get; set; }

        public SourceDTO(SourceType type, string url)
        {
            Type = type;
            URL = url;
        }
    }
}
