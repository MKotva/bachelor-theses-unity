using RegexTest.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RegexTest.SemanticAnalyzer;

namespace RegexTest.ExpressionItems
{
    public class MethodItem : Item
    {
        public string FunctionName;
        public List<List<Item>> Arguments;

        public override string ToString()
        {
            if(Arguments.Count == 0)
            {
                return $"Function {FunctionName}: Zero args.";
            }

            string output = $"Function {FunctionName}:\n";
            for(int i = 0; i < Arguments.Count; i++)
            {
                foreach(var item in Arguments[i])
                {
                    if (item.Type != ExpressionItemType.Function)
                        output += $"<Argument index: {i}-{item.Type}-{((ExpressionItem)item).ValueType}> {item.Content}\n";
                    else
                        output +=  $"<Argument index: {i}-{item.Type}> {item.Content}\n";
                }
            }
            return output;
        }
    }
}
