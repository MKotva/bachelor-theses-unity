using RegexTest.Enums;
using RegexTest.ExpressionItems;
using System;

namespace RegexTest.Semantic
{
    public class ActionItem
    {
        public ActionType ActionType = default;
        public string Content = default;
        public string Variable;
        public Value.VType VariableType;
        public Item[] Expression = default;

        public void Report()
        {
            Console.WriteLine(Content);
            Console.WriteLine($"    ActionType: {ActionType}");
            Console.WriteLine($"    Type:       {VariableType}");

            if (Variable != null)
                Console.WriteLine($"    Variable:   {Variable}");

            if (Expression != null)
            {
                Console.WriteLine($"    Expression:");
                foreach (var item in Expression)
                {
                    Console.WriteLine($"        {item}");
                }
            }
        }
    }
}
