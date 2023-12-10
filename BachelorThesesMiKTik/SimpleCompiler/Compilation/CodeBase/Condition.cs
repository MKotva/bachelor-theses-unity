using RegexTest.CodeBase;
using RegexTest.Enums;
using RegexTest.ExpressionEvaluator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegexTest.DTOS
{
   public class Condition : CodeLine
    {
        public TreeNode ConditionExpression { get; set; }
        public List<CodeLine> CodeLines { get; set; }
        public int LineNumber { get; set; }

        public Condition(TreeNode condition, List<CodeLine> code, int lineNumber = 0)
        {
            ConditionExpression = condition;
            CodeLines = code;
            LineNumber = lineNumber;
        }

        public bool Execute()
        {
            var cond = ConditionExpression.Evaluate();
            if (cond.Type != Value.VType.Boolean)
            {
                throw new Exception($"Invalid if condition! Expression is type of {cond.Type}");
            }

            return (bool) cond.Value;
        }

        void CodeLine.Execute()
        {
            throw new NotImplementedException();
        }
    }
}
