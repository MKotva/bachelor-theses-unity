using Assets.Core.SimpleCompiler.Enums;

namespace Assets.Core.SimpleCompiler.Compilation.ExpressionItems
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
