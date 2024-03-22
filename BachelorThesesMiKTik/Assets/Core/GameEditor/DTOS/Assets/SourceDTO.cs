using Assets.Core.GameEditor.Enums;
using System;

namespace Assets.Core.GameEditor.DTOS.Assets
{
    [Serializable] 
    public class SourceDTO
    {
        public string Name;
        public SourceType Type;
        public float XSize;
        public float YSize;

        public SourceDTO(string name, SourceType type, float xSize = 30, float ySize = 30) 
        {
            Name = name;
            Type = type;
            XSize = xSize;
            YSize = ySize;
        }
    }
}
