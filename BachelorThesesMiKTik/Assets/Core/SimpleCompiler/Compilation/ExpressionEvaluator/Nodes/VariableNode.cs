using System;
using Assets.Core.SimpleCompiler.Compilation.CodeBase;
using Assets.Core.SimpleCompiler.Compilation.ExpressionItems;

namespace Assets.Core.SimpleCompiler.Compilation.ExpressionEvaluator.Nodes
{
    public class VariableNode : TreeNode
    {
        private Property property;
        private CodeContext context;
        private string name;

        public VariableNode(CodeContext context, string variableName)
        {
            name = variableName;
            this.context = context;
            TryFindProperty(context, variableName);
        }
        public override Operand Evaluate()
        {
            if(property != null)
            {
                return property.Get();
            }
            else if (context.LocalVariables.ContainsKey(name))
            {
                return context.LocalVariables[name];
            }

            throw new Exception($"Undeclared variable {name}!");
        }

        private void TryFindProperty(CodeContext context, string variableName)
        {
            if (variableName.Contains(".")) 
            {
                property = context.GetProperty(variableName);
            }
        }
    }
}
