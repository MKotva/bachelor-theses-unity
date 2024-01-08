using Assets.Core.SimpleCompiler.Compilation.CodeBase;
using Assets.Core.SimpleCompiler.Compilation.ExpressionItems;
using Assets.Core.SimpleCompiler.Exceptions;

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

        /// <summary>
        /// If property was found in compilation, than this method will use it.
        /// Else the method will try to find variable name in global or local variables.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="RuntimeException"></exception>
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
            else if (context.GlobalVariables.ContainsKey(name)) 
            {
                return context.GlobalVariables[name];
            }

            throw new RuntimeException($"Undeclared variable {name}!");
        }

        /// <summary>
        /// If variable name is "path" to property (e.g. name.name.property), it must contain
        /// at least one '.', because declaring of property is not permited. So it must be connected
        /// to an object. So if path contains '.', this method will try to find it and set it as property
        /// to this class.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="variableName"></param>
        private void TryFindProperty(CodeContext context, string variableName)
        {
            if (variableName.Contains(".")) 
            {
                property = context.GetProperty(variableName);
            }
        }
    }
}
