using RegexTest.CodeBase;
using RegexTest.DTOS;
using RegexTest.ExpressionItems;
using System.Collections.Generic;

namespace RegexTest.ExpressionEvaluator.Nodes
{
    public class MethodNode : TreeNode
    {
        private List<TreeNode> Arguments;
        private Method Method;

        public MethodNode(CodeContext context, string functionName, List<TreeNode> arguments = null)
        {
            Arguments = arguments;

            var argumentCount = arguments.Count;
            if (arguments == null)
            {
                Arguments = new List<TreeNode>();
                argumentCount = 0;
            }

            Method = context.GetMethod(functionName, argumentCount);
        }

        public override Operand Evaluate()
        {
            var argumentResults = new List<object>();
            foreach (var argument in Arguments)
            {
                argumentResults.Add(argument.Evaluate().Value);
            }

            return Method.Invoke(argumentResults.ToArray());
        }
    }
}
