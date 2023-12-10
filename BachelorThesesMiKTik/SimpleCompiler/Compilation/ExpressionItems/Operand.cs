using ValueType = RegexTest.Enums.Value.VType;

namespace RegexTest.ExpressionItems
{
    public class Operand
    {
        public object Value { get; set; }
        public ValueType Type { get; set; }

        public Operand(object value, ValueType type)
        {
            Value = value;
            Type = type;
        }
    }
}
