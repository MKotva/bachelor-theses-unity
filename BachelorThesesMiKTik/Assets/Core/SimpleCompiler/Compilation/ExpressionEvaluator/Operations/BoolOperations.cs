using Assets.Core.SimpleCompiler.Compilation.ExpressionItems;
using Assets.Core.SimpleCompiler.Enums;
using Assets.Core.SimpleCompiler.Exceptions;

namespace Assets.Core.SimpleCompiler.Compilation.ExpressionEvaluator.Operations
{
    internal class BoolOperations
    {
        public static Operand Evaluate(Operator op, bool a, bool b)
        {
            switch (op.OperatorText)
            {
                case "==": return new Operand(a == b, ValueType.Boolean);
                case "!=": return new Operand(a != b, ValueType.Boolean);
                case "&" : return new Operand(a & b, ValueType.Boolean);
                case "^" : return new Operand(a ^ b, ValueType.Boolean);
                case "|" : return new Operand(a | b, ValueType.Boolean);
                case "||": return new Operand(a || b, ValueType.Boolean);
                case "&&": return new Operand(a && b, ValueType.Boolean);
                case "=": return new Operand(b, ValueType.Boolean);
                default:
                    throw new CompilationException($"Invalid operation between num {a} and num {b}");
            }
        }
    }
}
