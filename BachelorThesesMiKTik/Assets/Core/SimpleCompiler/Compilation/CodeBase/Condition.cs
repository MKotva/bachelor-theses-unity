using System.Collections.Generic;
using Assets.Core.SimpleCompiler.Enums;
using Assets.Core.SimpleCompiler.Compilation.ExpressionEvaluator.Nodes;
using Assets.Core.SimpleCompiler.Exceptions;

namespace Assets.Core.SimpleCompiler.Compilation.CodeBase
{
   public class Condition
    {
        public TreeNode ConditionExpression { get; set; }
        public List<ICodeLine> CodeLines { get; set; }
        public int LineNumber { get; set; }

        public Condition(TreeNode condition, List<ICodeLine> code, int lineNumber = 0)
        {
            ConditionExpression = condition;
            CodeLines = code;
            LineNumber = lineNumber;
        }

        public bool Execute()
        {
            var cond = ConditionExpression.Evaluate();
            if (cond.Type != ValueType.Boolean)
            {
                throw new RuntimeException($"Invalid if condition! Expression is type of {cond.Type}");
            }

            return (bool) cond.Value;
        }
    }
}
