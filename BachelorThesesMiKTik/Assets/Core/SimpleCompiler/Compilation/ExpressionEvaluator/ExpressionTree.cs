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

            throw new CompilationException("Invalid number of operators!");
        }

        /// <summary>
        /// Checks given expression members for unary operators '-','!' and marks them as unary.
        /// </summary>
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

        /// <summary>
        /// Based on expression member type, this method will create Expression tree node.
        /// </summary>
        /// <exception cref="SyntaxException"></exception>
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
                        throw new SyntaxException($"Uknown element(syntaxt fail): {expr.Content}");
                }
            }
        }

        /// <summary>
        /// Creates expression tree value node from expression member
        /// </summary>
        /// <param name="expr">Expression member</param>
        /// <exception cref="SyntaxException">Exeption for value parsing error</exception>
        private void AddValue(ExpressionItem expr)
        {
            if (expr.ValueType == ValueType.ERROR || expr.ValueType == ValueType.Empty)
            {
                throw new SyntaxException($"Value parsing error in value {expr.Content}");
            }
            operands.Push(new OperandNode(new Operand(expr.Value, expr.ValueType)));
        }

        /// <summary>
        /// Creates expression tree function node from expression member
        /// </summary>
        /// <param name="func">Expression member</param>
        /// <exception cref="SyntaxException">Exception for name or argument parsing</exception>
        private void AddFunction(MethodItem func)
        {
            if (func.FunctionName == "" || func.Arguments == null)
            {
                throw new SyntaxException($"Function parsing error! Function name or argument parsing went wrong! Name: {func.FunctionName}");
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

        /// <summary>
        /// Creates expression tree variable node from expression member
        /// </summary>
        /// <param name="expr">Expression member</param>
        private void AddVariable(ExpressionItem expr)
        {
            operands.Push(new VariableNode(context, expr.Content));
        }

        /// <summary>
        /// Creates expression tree operator node. If operator has higher priority than
        /// last added operator, creates new expression tree(subtree) from previously added
        /// tree nodes.
        /// </summary>
        /// <param name="opText">Operator string representation</param>
        /// <param name="type">None,Bracket,Unary,Binary,Assign</param>
        /// <exception cref="CompilationException">Exception for uknown operator</exception>
        private void AddOperator(string opText, OperatorType type)
        {
            var op = new Operator(opText, type);
            while (operators.Count != 0)
            {
                if (op.Priority == 0)
                    throw new CompilationException($"Invalid operator {opText} exception!");

                if (operators.Peek().Priority >= op.Priority)
                    break;
                AddOperation();
            }
            operators.Push(op); // Push operator;
        }

        /// <summary>
        /// If close parenthesse is found, this method will create expression tree(subtree) from
        /// all nodes between open and close parenthesse.
        /// </summary>
        /// <exception cref="CompilationException">Exception for parethesse without pair.</exception>
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

        /// <summary>
        /// Creates expression tree operatio node from two operand nodes(Variable, Funciton etc.) and 
        /// one operator node.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="CompilationException">Exception for insufficient number of operands or operators.</exception>
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
