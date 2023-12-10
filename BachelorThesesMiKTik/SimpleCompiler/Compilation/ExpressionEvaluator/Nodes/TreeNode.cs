using RegexTest.ExpressionItems;

namespace RegexTest.ExpressionEvaluator
{
    public enum NodeType
    {
        None, Number, Operation
    }

    public abstract class TreeNode
    {
        public NodeType Type { get; set; }
        public abstract Operand Evaluate();
    }
}
