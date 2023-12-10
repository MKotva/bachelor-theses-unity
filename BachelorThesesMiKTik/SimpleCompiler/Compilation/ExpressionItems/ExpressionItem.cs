using RegexTest.Enums;
using ValueType = RegexTest.Enums.Value.VType;

namespace RegexTest.ExpressionItems
{
    public class ExpressionItem : Item
    {
        public ValueType ValueType;
        public object Value;

        public override string ToString()
        {
            if (Type == ExpressionItemType.Value)
                return $"<{Type}-{ValueType}> {Content}";
            else
                return $"<{Type}> {Content}";
        }
    }
}
