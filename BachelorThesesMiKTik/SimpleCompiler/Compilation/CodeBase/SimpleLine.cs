using RegexTest.ExpressionEvaluator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace RegexTest.CodeBase
{
    class SimpleLine : CodeLine
    {
        public int LineNumber { get; set; }

        private TreeNode expr;
        public SimpleLine(CodeContext context, TreeNode expression) 
        {
            if(expression == null) 
            {
                throw new Exception($"Parsing expression error at line {LineNumber}!");
            }
            expr = expression;
        }

        public void Execute()
        {
            expr.Evaluate();
        }
    }
}
