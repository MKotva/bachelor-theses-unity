using Assets.Core.SimpleCompiler.Enums;
using Assets.Core.SimpleCompiler.Compilation.ExpressionItems;
using Assets.Core.SimpleCompiler.Exceptions;

namespace Assets.Core.SimpleCompiler.Compilation.ExpressionEvaluator.Operations
{
    static class NumOperations
    {
        public static Operand Evaluate(Operator op, float a, float b)
        {
            switch (op.OperatorText)
            {
                case "+": return new Operand(a + b, ValueType.Numeric);
                case "-": return new Operand(a - b, ValueType.Numeric);
                case "*": return new Operand(a * b, ValueType.Numeric);
                case "/": return Divide(a, b);
                case "%": return Modulo(a, b);
                case "<<": return LeftShift(a, b);
                case ">>": return RightShift(a, b);
                case ">": return new Operand(a > b, ValueType.Boolean);
                case ">=": return new Operand(a >= b, ValueType.Boolean);
                case "<": return new Operand(a < b, ValueType.Boolean);
                case "<=": return new Operand(a <= b, ValueType.Boolean);
                case "==": return new Operand(a == b, ValueType.Boolean);
                case "!=": return new Operand(a != b, ValueType.Boolean);
                case "&": return BitwiseAnd(a, b);
                case "^": return BitwiseXOR(a, b);
                case "|": return BitwiseOR(a, b);
                case "=": return new Operand(b, ValueType.Numeric);
                case "+=": return new Operand(a + b, ValueType.Numeric);
                case "-=": return new Operand(a - b, ValueType.Numeric);
                case "*=": return new Operand(a * b, ValueType.Numeric);
                case "/=": return Divide(a, b);
                default:
                    throw new CompilationException($"Invalid operation between num {a} and num {b}");
            }
        }

        private static Operand Divide(float a, float b)
        {
            if (b == 0)
                throw new CompilationException("Zero division."); //TODO: Division by zero.

            return new Operand(a / b, ValueType.Numeric);
        }
        private static Operand Modulo(float a, float b)
        {
            if (b == 0)
                throw new CompilationException("Zero division."); //TODO: Division by zero.

            return new Operand(a % b, ValueType.Numeric);
        }

        private static Operand LeftShift(float a, float b)
        {
            return new Operand((float)((int)a << (int)b), ValueType.Numeric);
        }

        private static Operand RightShift(float a, float b)
        {
            return new Operand((float)((int) a >> (int) b), ValueType.Numeric);
        }

        private static Operand BitwiseAnd(float a, float b)
        {
            return new Operand((float)((int) a & (int) b), ValueType.Numeric);
        }
        private static Operand BitwiseXOR(float a, float b)
        {
            return new Operand((float)((int) a ^ (int) b), ValueType.Numeric);
        }
        private static Operand BitwiseOR(float a, float b)
        {
            return new Operand((float)((int) a | (int) b), ValueType.Numeric);
        }
    }
}
