using RegexTest.DTOS;
using RegexTest.Enums;
using RegexTest.ExpressionEvaluator;
using RegexTest.ExpressionEvaluator.Operations;
using RegexTest.ExpressionItems;
using System;

namespace RegexTest.CodeBase
{
    public class AssignLine : CodeLine
    {
        public int LineNumber { get; set; }

        private CodeContext context;
        private TreeNode expr;
        private Property property;
        private string variableName;
        private Value.VType variableType;
        private Operator op;

        public AssignLine(CodeContext context, TreeNode expression, string assingVar, Value.VType varType = Value.VType.Empty, string assignType = "=")
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
                        throw new Exception($"Invalid operator {op.OperatorText} for {variableName} variable declaration");
                    }
                    context.LocalVariables.Add(variableName, value);
                }
                else
                {
                    if (variableType == Value.VType.Empty || variableType == Value.VType.ERROR)
                    {
                        context.LocalVariables[variableName] = ParseAssign(context.LocalVariables[variableName], value);
                    }
                    else
                    {
                        throw new Exception($"Redeclaring existing variable {variableName}!");
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
