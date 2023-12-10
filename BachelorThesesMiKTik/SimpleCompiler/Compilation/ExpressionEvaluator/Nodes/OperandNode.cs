using RegexTest.ExpressionItems;

namespace RegexTest.ExpressionEvaluator
{
    public class OperandNode : TreeNode
    {
        Operand Value { get; set; }

        public OperandNode(Operand value)
        {
            Value = value;
        }

        public override Operand Evaluate()
        {
            return Value;
        }
    }
}
