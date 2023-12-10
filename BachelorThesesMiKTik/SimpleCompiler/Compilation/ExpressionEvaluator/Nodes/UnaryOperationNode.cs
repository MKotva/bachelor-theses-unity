using RegexTest.ExpressionItems;
using System;
using ValueType = RegexTest.Enums.Value.VType;

namespace RegexTest.ExpressionEvaluator
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
                    throw new Exception("Invalid unary operator!");
            }
        }

        private Operand AritmeticNegation(TreeNode operand)
        {
            var value = operand.Evaluate();
            if (value.Type != ValueType.Numeric)
                throw new Exception("Unary operator \"-\" invalid for non num types.");

            return new Operand(-1 * (float)value.Value, ValueType.Numeric);
        }

        private Operand BooleanNegation(TreeNode operand)
        {
            var value = operand.Evaluate();
            if (value.Type != ValueType.Boolean)
                throw new Exception("Unary operator \"!\" invalid for non bool types.");
     
            return new Operand(!(bool)value.Value, ValueType.Boolean);
        }
    }
}
