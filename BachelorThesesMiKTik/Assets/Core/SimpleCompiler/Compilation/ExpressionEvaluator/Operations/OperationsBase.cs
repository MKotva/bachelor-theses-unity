using Assets.Core.SimpleCompiler.Compilation.ExpressionItems;
using Assets.Core.SimpleCompiler.Enums;
using Assets.Core.SimpleCompiler.Exceptions;

namespace Assets.Core.SimpleCompiler.Compilation.ExpressionEvaluator.Operations
{
    public static class OperationsBase
    {
        public static Operand PerformOperation(Operator op, Operand a, Operand b)
        {
            if (a.Type != b.Type)
                throw new CompilationException($"Inconsistent types! {a.Type} {op.OperatorText} {b.Type}");

            switch (a.Type)
            {
                case ValueType.Boolean: return BoolOperations.Evaluate(op, ((bool) a.Value), ((bool) b.Value));
                case ValueType.Numeric: return NumOperations.Evaluate(op, ParseNumeric(a.Value), ParseNumeric(b.Value));
                case ValueType.String: return StringOperations.Evaluate(op, (string) a.Value, ((string) b.Value));
                case ValueType.Empty:
                    throw new CompilationException($"Invalid operation between non-returning function and operand.");
                default:
                    throw new CompilationException("Ivalid operand type!");
            }
        }

        private static float ParseNumeric(object a)
        {
            if (a is int)
                return ((int) a);
            else if(a is long)
                return ((long) a);
            else if(a is double)
                return (float)((double)a);
            else if(a is float)
                return (float)a;
            throw new CompilationException($"Unsupported variable type: {a.GetType().Name}");
        }
    }
}
