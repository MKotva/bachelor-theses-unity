using RegexTest.ExpressionItems;
using System;
using ValueType = RegexTest.Enums.Value.VType;

namespace RegexTest.ExpressionEvaluator.Operations
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
                    throw new Exception($"Invalid operation between num {a} and num {b}");
            }
        }
    }
}
