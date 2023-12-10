using System;

namespace Assets.Core.GameEditor.ExpressionEvaluator
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

        public override float Evaluate()
        {
            var lvalue = left.Evaluate();
            var rvalue = right.Evaluate();

            switch (op.OperatorText) 
            {
                case "+" : return lvalue + rvalue;
                case "-" : return lvalue - rvalue;
                case "*" : return lvalue * rvalue;
                case "/" : return Divide(lvalue, rvalue);
                case "%" : return Modulo(lvalue, rvalue);
                case "<<": return LeftShift(lvalue, rvalue);
                case ">>": return RightShift(lvalue, rvalue);
                case "==": return Equal(lvalue, rvalue);
                case "!=": return NotEqual(lvalue, rvalue);
                case "&" : return BitwiseAnd(lvalue, rvalue);
                case "^" : return BitwiseXOR(lvalue, rvalue);
                case "|" : return BitwiseOR(lvalue, rvalue);
                case "||": return OR(lvalue, rvalue);
                case "&&": return AND(lvalue, rvalue);
                default:
                    //TODO: Invalid op exception
                    return 0;
            }
        }

        private static float Divide(float a, float b)
        {
            if (b == 0)
                return 0; //TODO: Division by zero.

            return a / b;
        }
        private static float Modulo(float a, float b)
        {
            if (b == 0)
                return 0; //TODO: Division by zero.

            return a % b;
        }
        private static float LeftShift(float a, float b)
        {
            return (int)a << (int)b;
        }
        private static float RightShift(float a, float b)
        {
            return (int) a >> (int) b;
        }
        private static float Equal(float a, float b)
        {
            if (a == b)
                return 1;
            return 0;
        }
        private static float NotEqual(float a, float b)
        {
            if (a != b)
                return 1;
            return 0;
        }
        private static float BitwiseAnd(float a, float b)
        {
            return (int)a & (int)b;
        }
        private static float BitwiseXOR(float a, float b)
        {
            return (int)a ^ (int)b;
        }
        private static float BitwiseOR(float a, float b)
        {
            return (int)a | (int)b;
        }
        private static float AND(float a, float b)
        {
            if(a * b > 0)
                return 1;
            return 0; 
        }
        private static float OR(float a, float b)
        {
            if (a + b > 0)
                return 1;
            return 0;
        }
    }
}
