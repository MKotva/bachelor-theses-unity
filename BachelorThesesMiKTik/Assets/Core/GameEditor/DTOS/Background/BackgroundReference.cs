using Assets.Core.GameEditor.DTOS.Assets;
using Assets.Core.GameEditor.Enums;
using Newtonsoft.Json;
using System;

namespace Assets.Core.GameEditor.DTOS.Background
{
    [Serializable]
    public class BackgroundReference : SourceReference
    {
        public float ParalaxSpeed = 0f;

        [JsonConstructor]
        public BackgroundReference(string name, SourceType type, float xSize = 30, float ySize = 30, float paralax = 0)
            : base(name, type, xSize, ySize)
        {
            ParalaxSpeed = paralax;
        }

        public BackgroundReference(SourceReference sourceReference, float paralax)
            : base(sourceReference.Name, sourceReference.Type, sourceReference.XSize, sourceReference.YSize) 
        {
            ParalaxSpeed = paralax;
        }
    }
}
