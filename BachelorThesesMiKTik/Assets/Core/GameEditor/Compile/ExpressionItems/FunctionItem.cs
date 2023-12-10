using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Assets.Core.GameEditor.Compile.Interpreter;

namespace Assets.Core.GameEditor.Compile.ExpressionItems
{
    public class FunctionItem : ExpressionItem
    {
        public string FunctionName;
        public List<List<ExpressionItem>> Arguments;

        public override string ToString()
        {
            if (Arguments.Count == 0)
            {
                return $"Function {FunctionName}: Zero args.";
            }

            string output = $"Function {FunctionName}:\n";
            for (int i = 0; i < Arguments.Count; i++)
            {
                foreach (var item in Arguments[i])
                {
                    if (Type == ExpressionItemType.Value)
                        output += $"<Argument index: {i}-{Type}-{ValueType}> {Content}\n";
                    else
                        output += $"<Argument index: {i}-{Type}> {Content}\n";
                }
            }
            return output;
        }
    }
}
