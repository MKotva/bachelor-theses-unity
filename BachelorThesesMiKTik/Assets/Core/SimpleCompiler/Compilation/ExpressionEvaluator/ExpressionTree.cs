using System.Collections.Generic;
using Assets.Core.SimpleCompiler.Compilation.CodeBase;
using Assets.Core.SimpleCompiler.Compilation.ExpressionEvaluator.Nodes;
using Assets.Core.SimpleCompiler.Compilation.ExpressionItems;
using Assets.Core.SimpleCompiler.Enums;
using Assets.Core.SimpleCompiler.Exceptions;

namespace Assets.Core.SimpleCompiler.Compilation.ExpressionEvaluator
{
    public class ExpressionTree
    {
        public IEnumerable<Item> Expressions { get; private set; }

        private CodeContext context;
        private Stack<TreeNode> operands;
        private Stack<Operator> operators;

        public ExpressionTree(IEnumerable<Item> expression, CodeContext codeContext)
        {
            Expressions = expression;
            context = codeContext;
            operands = new Stack<TreeNode>();
            operators = new Stack<Operator>();
        }

        public TreeNode Build()
        {
            CheckForUnaryOperators();
            ParseExpressions();
            while (operators.Count != 0)
            {
                AddOperation();
            }

            if (operands.Count == 1)
                return operands.Pop();

            return null;
        }

        private void CheckForUnaryOperators()
        {
            Item previous = new Item() { Type = ExpressionItemType.Operator };
            foreach (var expr in Expressions)
            {
                if (expr.Type == ExpressionItemType.Operator &&
                    ( expr.Content == "-" || expr.Content == "!" ))
                {
                    if (previous.Type == ExpressionItemType.Operator ||
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
                switch (expr.Type)
                {
                    case ExpressionItemType.Value:
                        AddValue((ExpressionItem)expr);
                        break;
                    case ExpressionItemType.Variable:
                        AddVariable((ExpressionItem)expr);
                        break;
                    case ExpressionItemType.Function:
                        AddFunction((MethodItem) expr);
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
                }
            }
        }

        private void AddValue(ExpressionItem expr)
        {
            if (expr.ValueType == ValueType.ERROR || expr.ValueType == ValueType.Empty)
            {
                throw new CompilationException($"Value parsing error in value {expr.Content}");
            }
            operands.Push(new OperandNode(new Operand(expr.Value, expr.ValueType)));
        }

        private void AddFunction(MethodItem func)
        {
            if (func.FunctionName == "" || func.Arguments == null)
            {
                throw new CompilationException($"Function parsing error! Function name or argument parsing went wrong! Name: {func.FunctionName}");
            }

            var arguments = new List<TreeNode>();
            foreach (var argExpr in func.Arguments)
            {
                var expression = new ExpressionTree(argExpr, context);
                var argument = expression.Build();
                arguments.Add(argument);
            }

            operands.Push(new MethodNode(context, func.FunctionName, arguments));
        }

        private void AddVariable(ExpressionItem expr)
        {
            operands.Push(new VariableNode(context, expr.Content));
        }

        private void AddOperator(string opText, OperatorType type)
        {
            var op = new Operator(opText, type);
            while (operators.Count != 0)
            {
                if (op.Priority == 0)
                    return;

                if (operators.Peek().Priority >= op.Priority)
                    break;
                AddOperation();
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
                AddOperation();
            }

            if (!pairFound)
            {
                throw new CompilationException("Inconsistent number of parenthesees!");
            }
            operators.Pop();
        }

        private void AddOperation()
        {
            var operation = CreateOperation();
            operands.Push(operation);
        }
        private TreeNode CreateOperation()
        {
            if (operators.Count < 1)
            {
                throw new CompilationException($"The expression lacks required number of operators!");
            }
            var op = operators.Pop();
            if (op.OperatorType == OperatorType.Unary && operands.Count >= 1)
            {
                return new UnaryOperationNode(op, operands.Pop());
            }
            else if (op.OperatorType == OperatorType.Binary && operands.Count >= 2)
            {
                var a = operands.Pop();
                var b = operands.Pop();
                return new OperationNode(op, b, a);
            }

            throw new CompilationException($"The expression lacks required number of operands for operator {operators.Peek().OperatorText}!");
        }
    }
}
