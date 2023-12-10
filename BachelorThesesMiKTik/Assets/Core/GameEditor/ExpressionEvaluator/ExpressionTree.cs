using Assets.Core.GameEditor.Compile.ExpressionItems;
using System;
using System.Collections.Generic;
using static Assets.Core.GameEditor.Compile.Interpreter;

namespace Assets.Core.GameEditor.ExpressionEvaluator
{
    public class ExpressionTree
    {
        public ExpressionItem[] Expressions { get; private set; }

        private Dictionary<string, float> variables;
        private Stack<TreeNode> operands;
        private Stack<Operator> operators;

        public ExpressionTree(ExpressionItem[] expression, Dictionary<string, float> variables)
        {
            Expressions = expression;
            this.variables = variables;
            operands = new Stack<TreeNode>();
            operators = new Stack<Operator>();
        }

        public TreeNode Build()
        {
            CheckForUnaryOperators();
            ParseExpressions();
            while (operators.Count != 0)
            {
                if (!TryAddOperation())
                {
                    //TODO: Exception handle
                    return null;
                }
            }

            if (operands.Count == 1)
                return operands.Pop();

            return null;
        }

        private void CheckForUnaryOperators()
        {
            ExpressionItem previous = new ExpressionItem() { Type = ExpressionItemType.Operator};
            foreach (var expr in Expressions)
            {
                if (expr.Type == ExpressionItemType.Operator &&
                    ( expr.Content == "-" || expr.Content == "!"))
                {
                    if(previous.Type == ExpressionItemType.Operator ||
                        previous.Type == ExpressionItemType.OpenBracket)
                    {
                        expr.Type = ExpressionItemType.UnaryOperator;
                    }
                }

                previous = expr;
            }
        }

        private void ParseExpressions()
        {
            foreach (var expr in Expressions)
            {
                switch(expr.Type)
                {
                    case ExpressionItemType.Value:
                        AddValue(expr);
                        break;
                    case ExpressionItemType.Variable:
                        AddVariable(expr);
                        break;
                    case ExpressionItemType.OpenBracket:
                        operators.Push(new Operator("(", OperatorType.Bracket));
                        break;
                    case ExpressionItemType.CloseBracket:
                        EvaluateBracket();
                        break;
                    case ExpressionItemType.UnaryOperator:
                        AddOperator(expr.Content, OperatorType.Unary);
                        break;
                    case ExpressionItemType.Operator:
                        AddOperator(expr.Content, OperatorType.Binary);
                        break;
                    default:
                        break;
                };     
            }
        }

        private void AddValue(ExpressionItem expr)
        {
            if (expr.ValueType == Compile.Interpreter.ValueType.ERROR)
            {
                return; //TODO: Parsing error Exception
            }
            else if (expr.ValueType == Compile.Interpreter.ValueType.Numeric ||
                     expr.ValueType == Compile.Interpreter.ValueType.Boolean)
            {
                float value = expr.Value ?? 0;
                operands.Push(new OperandNode(value));
            }
        }

        private void AddVariable(ExpressionItem expr)
        {
            //if (!variables.ContainsKey(expr.Content))
            //{
            //    return; //TODO:Undeclared variable exception.
            //}
            operands.Push(new VariableNode(expr.Content, variables));
        }

        private void AddOperator(string opText, OperatorType type)
        {
            var op = new Operator(opText, type);
            while (operators.Count != 0)
            {
                if (op.Priority == 0)
                    return;

                if(operators.Peek().Priority >= op.Priority)
                    break;
                if (!TryAddOperation())
                {
                    //Exception handle
                }
            }
            operators.Push(op); // Push operator;
        }

        private void EvaluateBracket()
        {
            bool pairFound = false;
            while (operators.Count != 0)
            {
                if (operators.Peek().OperatorText == "(")
                {
                    pairFound = true;
                    break;
                }
                if (!TryAddOperation())
                {
                    return; //TODO: Exception handle
                }
            }

            if (!pairFound)
            {
                return; //TODO: Exception handle
            }
            operators.Pop();
        }

        private bool TryAddOperation()
        {
            var operation = CreateOperation();
            if (operation == null)
                return false;

            operands.Push(operation);
            return true;
        }
        private TreeNode CreateOperation()
        {
            if (operators.Count < 1)
            {
                return null;
            }
            var op = operators.Pop();
            if(op.OperatorType == OperatorType.Unary && operands.Count >= 1)
            {
                return new UnaryOperationNode(op, operands.Pop());
            }
            else if(op.OperatorType == OperatorType.Binary && operands.Count >= 2)
            {
                var a = operands.Pop();
                var b = operands.Pop();
                return new OperationNode(op, b, a);
            }
            else
            {
                //TODO: Invalid number of arguments exception.
                return null;
            }
        }

        private void Error() { throw new NotImplementedException(); }
    }
}
