using Assets.Core.GameEditor.ExpressionEvaluator;
using System.Collections.Generic;

namespace Assets.Core.GameEditor.CodeBase
{
    public class AssignLine : ICodeLine
    {
        private Dictionary<string, float> vars;
        private TreeNode expr;
        private string varName;
        private string assignType;


        public AssignLine(Dictionary<string, float> variables, TreeNode expression, string assingVar, string assignType = "=") 
        {
            vars = variables;
            expr = expression;
            varName = assingVar;   
            this.assignType = assignType;
        }

        public void Execute()
        {
            var value = expr.Evaluate();

            if (!vars.ContainsKey(varName))
            {
                vars.Add(varName, value);
            }
            else
            {
                switch(assignType)
                {
                    case "=": vars[varName] = value;
                        break;
                }
                vars[varName] = value;
            }
        }
    }
}
