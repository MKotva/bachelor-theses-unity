
using Assets.Core.SimpleCompiler.Enums;

namespace Assets.Core.GameEditor.DTOS
{
    public class GlobalVariableDTO
    {
        public string Alias { get; set; }
        public string Value { get; set; }
        public ValueType Type { get; set; }

        public GlobalVariableDTO(string alias, string value, ValueType valueType) 
        {
            Alias = alias;
            Value = value;
            Type = valueType;
        }
    }
}
