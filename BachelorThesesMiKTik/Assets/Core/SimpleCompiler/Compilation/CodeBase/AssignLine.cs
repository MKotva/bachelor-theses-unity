using Assets.Core.SimpleCompiler.Enums;
using Assets.Core.SimpleCompiler.Compilation.ExpressionItems;
using Assets.Core.SimpleCompiler.Compilation.ExpressionEvaluator.Nodes;
using Assets.Core.SimpleCompiler.Compilation.ExpressionEvaluator.Operations;
using Assets.Core.SimpleCompiler.Exceptions;

namespace Assets.Core.SimpleCompiler.Compilation.CodeBase
{
    public class AssignLine : ICodeLine
    {
        public int LineNumber { get; set; }

        private CodeContext context;
        private TreeNode expr;
        private Property property;
        private string variableName;
        private ValueType variableType;
        private Operator op;

        public AssignLine(CodeContext context, TreeNode expression, string assingVar, ValueType varType = ValueType.Empty, string assignType = "=")
        {
            this.context = context;
            expr = expression;
            variableName = assingVar;
            variableType = varType;
            op = new Operator(assignType, OperatorType.Assign);

            TryFindProperty(context, variableName);
        }

        public void Execute()
        {
            var value = expr.Evaluate();

            if (property != null)
            {
                var result = ParseAssign(property.Get(), value);
                property.Set(result);
            }
            else
            {
                if (!context.LocalVariables.ContainsKey(variableName))
                {
                    if(op.OperatorText != "=") 
                    {
                        throw new RuntimeException($"Invalid operator {op.OperatorText} for {variableName} variable declaration");
                    }
                    context.LocalVariables.Add(variableName, value);
                }
                else
                {
                    if (variableType == ValueType.Empty || variableType == ValueType.ERROR)
                    {
                        context.LocalVariables[variableName] = ParseAssign(context.LocalVariables[variableName], value);
                    }
                    else
                    {
                        throw new RuntimeException($"Redeclaring existing variable {variableName}!");
                    }
                }    
            }
        }

        private void TryFindProperty(CodeContext context, string variableName)
        {
            if (variableName.Contains("."))
            {
                property = context.GetProperty(variableName);
            }
        }

        private Operand ParseAssign(Operand left, Operand right)
        {
            return OperationsBase.PerformOperation(op, left, right);
        }
    }
}
