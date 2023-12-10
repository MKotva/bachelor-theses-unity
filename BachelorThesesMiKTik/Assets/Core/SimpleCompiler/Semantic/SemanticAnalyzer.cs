using System.Collections.Generic;
using Assets.Core.SimpleCompiler.Enums;


namespace Assets.Core.SimpleCompiler.Semantic
{
    public class SemanticAnalyzer
    { 
        public ActionItem[] AnalyzeCode(string[] lines)
        {
            var result = new List<ActionItem>();
            foreach (var line in lines)
            {
                var action = AnalyzeLine(line.Trim());
                result.Add(action);
            }
            return result.ToArray();
        }

        private ActionItem AnalyzeLine(string line)
        {
            var result = new ActionItem();
            result.Content = line;

            var match = Patterns.LineRegex.Match(line);
            if (line.StartsWith("#") || match.Groups["empty"].Success)
            {
                result.ActionType = ActionType.EMPTY;
            }
            else if (match.Groups["if"].Success)
            {
                result.ActionType = ActionType.If;
                result.Expression = Expression.ParseExpression(match.Groups["if_expression"].Value);
            }
            else if (match.Groups["elseif"].Success)
            {
                result.ActionType = ActionType.ElseIf;
                result.Expression = Expression.ParseExpression(match.Groups["elseif_expression"].Value);
            }
            else if (match.Groups["else"].Success)
            {
                result.ActionType = ActionType.Else;
            }
            else if (match.Groups["fi"].Success)
            {
                result.ActionType = ActionType.Fi;
            }
            else if (match.Groups["while"].Success)
            {
                result.ActionType = ActionType.While;
                result.Expression = Expression.ParseExpression(match.Groups["while_expression"].Value);
            }
            else if (match.Groups["assignLine"].Success)
            {
                result.ActionType = ActionType.Assign;
                result.VariableType = ValueType.GetFromName(match.Groups["assignValue_type"].Value);
                result.Expression = Expression.ParseExpression(match.Groups["assignValue_expression"].Value);
                result.Variable = match.Groups["assignValue_variable"].Value;
            }
            else if (match.Groups["end"].Success)
            {
                result.ActionType = ActionType.End;
            }
            else if (match.Groups["simpleLine"].Success)
            {
                result.ActionType = ActionType.SimpleLine;
                result.Expression = Expression.ParseExpression(match.Groups["assignValue_expression"].Value);
            }
            else
            {
                result.ActionType = ActionType.ERROR;
            }

            return result;
        }
    }
}
