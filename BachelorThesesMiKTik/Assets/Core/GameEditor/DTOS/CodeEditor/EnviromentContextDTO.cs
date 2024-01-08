using System;

namespace Assets.Core.GameEditor.DTOS.CodeEditor
{
    public class EnviromentContextDTO
    {
        public object Instance { get; set; }
        public Type Type { get; set; }

        public EnviromentContextDTO(object instace, Type type) 
        {
            Instance = instace;
            Type = type;
        }
    }
}
