using Assets.Core.SimpleCompiler.Compilation.ExpressionItems;

namespace Assets.Core.SimpleCompiler.Compilation.ExpressionEvaluator.Nodes
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
