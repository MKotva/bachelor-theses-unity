using Assets.Core.SimpleCompiler.Compilation.ExpressionItems;
using Assets.Core.SimpleCompiler.Compilation.ExpressionEvaluator.Operations;

namespace Assets.Core.SimpleCompiler.Compilation.ExpressionEvaluator.Nodes
{
    public class OperationNode : TreeNode
    {
        private Operator op;
        private TreeNode left;
        private TreeNode right;

        public OperationNode(Operator op, TreeNode left, TreeNode right)
        {
            this.op = op;
            this.left = left;
            this.right = right;
        }

        public override Operand Evaluate()
        {
            var lvalue = left.Evaluate();
            var rvalue = right.Evaluate();

            return OperationsBase.PerformOperation(op, lvalue, rvalue);
        }
    }
}
