using System.Collections.Generic;

namespace Assets.Core.GameEditor.ExpressionEvaluator
{
    public class VariableNode : TreeNode
    {
        private string name;
        private Dictionary<string, float> vars;

        public VariableNode(string variableName, Dictionary<string, float> variables) 
        {
            name = variableName;
            vars = variables;
        }
        public override float Evaluate()
        {
            if(!vars.ContainsKey(name))
            {
                //TODO: Throw undeclared variable.
                return 0;
            }

            return vars[name];
        }
    }
}
