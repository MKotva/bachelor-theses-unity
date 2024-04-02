using Assets.Core.SimpleCompiler;
using System;
using System.Collections.Generic;

namespace Assets.Core.GameEditor.DTOS.SourcePanels
{
    [Serializable]
    public class CollisionDTO
    {
        public List<string> ObjectsNames;
        public List<string> GroupsNames;
        public SimpleCode Handler;

        public CollisionDTO(List<string> names, List<string> groups, SimpleCode handler) 
        {
            ObjectsNames = names;
            GroupsNames = groups;
            Handler = handler;
        }
    }
}
