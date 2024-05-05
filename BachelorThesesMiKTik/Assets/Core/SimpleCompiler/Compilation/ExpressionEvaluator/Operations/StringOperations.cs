using Assets.Core.SimpleCompiler.Enums;
using Assets.Core.SimpleCompiler.Exceptions;
using Assets.Core.SimpleCompiler.Compilation.ExpressionItems;

namespace Assets.Core.SimpleCompiler.Compilation.ExpressionEvaluator.Operations
{
    internal class StringOperations
    {
        public static Operand Evaluate(Operator op, string a, string b)
        {
            switch (op.OperatorText)
            {
                case "+": return new Operand(a + b, ValueType.String);
                case "==": return new Operand(a == b, ValueType.Boolean);
                case "!=": return new Operand(a != b, ValueType.Boolean);
                case "=": return new Operand(b, ValueType.String);
                case "+=": return new Operand(a + b, ValueType.String);
                default:
                    throw new CompilationException($"Invalid operation between num {a} and num {b}");
            }
        }
    }
}
