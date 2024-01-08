using Assets.Core.SimpleCompiler;
using System;

namespace Assets.Core.GameEditor.DTOS.SourcePanels
{
    [Serializable]
    public class CollisionDTO
    {
        public string ObjectName;
        public SimpleCode Handler;

        public CollisionDTO(string name, SimpleCode handler) 
        {
            ObjectName = name;
            Handler = handler;
        }
    }
}
