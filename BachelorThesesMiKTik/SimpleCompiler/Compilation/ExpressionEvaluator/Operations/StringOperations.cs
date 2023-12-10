using RegexTest.ExpressionItems;
using System;
using ValueType = RegexTest.Enums.Value.VType;

namespace RegexTest.ExpressionEvaluator.Operations
{
    internal class StringOperations
    {
        public static Operand Evaluate(Operator op, string a, string b)
        {
            switch (op.OperatorText)
            {
                case "+": return new Operand(a + b, ValueType.String);
                case "==": return new Operand(a == b, ValueType.String);
                case "!=": return new Operand(a != b, ValueType.String);
                case "=": return new Operand(b, ValueType.String);
                case "+=": return new Operand(a + b, ValueType.String);
                default:
                    throw new Exception($"Invalid operation between num {a} and num {b}");
            }
        }
    }
}
