using Assets.Core.SimpleCompiler.Compilation.ExpressionItems;
using Assets.Core.SimpleCompiler.Enums;
using Assets.Core.SimpleCompiler.Exceptions;

namespace Assets.Core.SimpleCompiler.Compilation.ExpressionEvaluator.Nodes
{
    public class UnaryOperationNode : TreeNode
    {
        private Operator op;
        private TreeNode operand;

        public UnaryOperationNode(Operator operation, TreeNode operand)
        {
            this.op = operation;
            this.operand = operand;
        }

        public override Operand Evaluate()
        {
            switch (op.OperatorText)
            {
                case "-": return AritmeticNegation(operand);
                case "!": return BooleanNegation(operand);
                default:
                    throw new CompilationException("Invalid unary operator!");
            }
        }

        private Operand AritmeticNegation(TreeNode operand)
        {
            var value = operand.Evaluate();
            if (value.Type != ValueType.Numeric)
                throw new CompilationException("Unary operator \"-\" invalid for non num types.");

            return new Operand(-1 * (float)value.Value, ValueType.Numeric);
        }

        private Operand BooleanNegation(TreeNode operand)
        {
            var value = operand.Evaluate();
            if (value.Type != ValueType.Boolean)
                throw new CompilationException("Unary operator \"!\" invalid for non bool types.");
     
            return new Operand(!(bool)value.Value, ValueType.Boolean);
        }
    }
}
