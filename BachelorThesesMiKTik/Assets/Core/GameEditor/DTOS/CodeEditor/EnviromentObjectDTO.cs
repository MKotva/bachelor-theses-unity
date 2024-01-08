using System;

namespace Assets.Core.GameEditor.DTOS
{
    [Serializable]
    public class EnviromentObjectDTO
    {
        public string Alias { get; set; }
        public string TypeName { get; set; }

        public EnviromentObjectDTO(string alias, string typeName) 
        {
            Alias = alias;
            TypeName = typeName;
        }
    }
}
