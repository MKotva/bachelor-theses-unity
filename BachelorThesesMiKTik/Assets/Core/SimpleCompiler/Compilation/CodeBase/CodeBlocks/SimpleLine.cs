using System;
using Assets.Core.SimpleCompiler.Compilation.ExpressionEvaluator.Nodes;

namespace Assets.Core.SimpleCompiler.Compilation.CodeBase
{
    class SimpleLine : ICodeLine
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
