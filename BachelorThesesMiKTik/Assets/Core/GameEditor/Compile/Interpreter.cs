using Assets.Core.GameEditor.Compile.ExpressionItems;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Assets.Core.GameEditor.Compile
{
    public class Interpreter
    {
        private static class Expression
        {
            public static readonly Regex expressionSplitRegex;
            private static readonly string[] expresionDelimiters = new[]
            {
                "&&", "||", "<=", ">=", "==", "!=", "<", ">",
                "+", "-", "*", "/", "%", "&", "|",
                " ", "(", ")", ","
            };

            static Expression()
            {
                string expressionSplitPattern = "(" + String.Join("|", expresionDelimiters.Select(x => Regex.Escape(x))) + ")";
                expressionSplitRegex = new Regex(expressionSplitPattern, RegexOptions.Compiled);
            }

            public static ExpressionItem[] ParseExpression(string expression)
            {
                var items = Expression.SplitExpression(expression);
                List<ExpressionItem> result = new List<ExpressionItem>();

                foreach (var item in items)
                {
                    var typematch = Patterns.ExpressionTerminalRegex.Match(item);
                    var res = new ExpressionItem
                    {
                        Content = item
                    };

                    if (!typematch.Success)
                        res.Type = ExpressionItemType.ERROR;
                    else if (typematch.Groups["boolOperator"].Success)
                        res.Type = ExpressionItemType.Operator;
                    else if (typematch.Groups["aritmeticOperator"].Success)
                        res.Type = ExpressionItemType.Operator;
                    else if (typematch.Groups["value"].Success)
                    {
                        res.Type = ExpressionItemType.Value;

                        var valueTypeMatch = Patterns.ValueTypeRegex.Match(item);
                        if (valueTypeMatch.Groups["booleanValue"].Success)
                        {
                            res.ValueType = ValueType.Boolean;
                            var boolean = bool.Parse(item);
                            if (boolean)
                            {
                                res.Value = 1;
                            }
                            else
                            {
                                res.Value = 0;
                            }
                        }
                        else if (valueTypeMatch.Groups["numericValue"].Success)
                        {
                            res.ValueType = ValueType.Numeric;
                            if (float.TryParse(item, NumberStyles.Any, CultureInfo.InvariantCulture, out float value))
                            {
                                res.Value = value;
                            }
                            else
                            {
                                //TODO: Value exception, invalid format.
                                res.Value = 0;
                            }
                        }
                        else if (valueTypeMatch.Groups["stringValue"].Success)
                        {
                            //res.ValueType = ValueType.Boolean;
                            //res.StringValue = item.Trim('"');
                        }
                        else
                        {
                            res.ValueType = ValueType.ERROR;
                        }


                    }
                    else if (typematch.Groups["variable"].Success)
                        res.Type = ExpressionItemType.Variable;
                    else if (typematch.Groups["openBracket"].Success)
                        res.Type = ExpressionItemType.OpenBracket;
                    else if (typematch.Groups["closeBracket"].Success)
                        res.Type = ExpressionItemType.CloseBracket;
                    else if (typematch.Groups["funcArgDelimeter"].Success)
                        res.Type = ExpressionItemType.ArgumentDelimeter;
                    else
                        res.Type = ExpressionItemType.ERROR;

                    result.Add(res);
                }

                result = FindFunctions(result);
                return result.ToArray();
            }

            private static List<ExpressionItem> FindFunctions(List<ExpressionItem> items)
            {
                var outItems = new List<ExpressionItem>();

                for (int index = 0; index < items.Count; index++)
                {
                    if (index + 1 < items.Count && IsFunction(items[index], items[index + 1]))
                    {
                        outItems.Add(ParseFunction(items, items[index].Content, index + 2, out index));
                    }
                    else
                    {
                        outItems.Add(items[index]);
                    }
                }

                return outItems;
            }

            private static FunctionItem ParseFunction(List<ExpressionItem> items, string name, int startIndex, out int newIndex)
            {
                var actualArg = new List<ExpressionItem>();
                var args = new List<List<ExpressionItem>> { };
                for (int index = startIndex; index < items.Count; index++)
                {
                    if (items[index].Type == ExpressionItemType.ArgumentDelimeter)
                    {
                        if (actualArg.Count == 0)
                        {
                            throw new Exception("Empty function argument");
                        }
                        else
                        {
                            args.Add(actualArg);
                            actualArg = new List<ExpressionItem>();
                        }
                    }
                    else if (items[index].Type == ExpressionItemType.CloseBracket)
                    {
                        if (args.Count != 0 && actualArg.Count == 0)
                        {
                            throw new Exception("Empty function argument");
                        }
                        else if (args.Count != 0)
                        {
                            args.Add(actualArg);
                        }
                        newIndex = index;
                        return new FunctionItem
                        {
                            Type = ExpressionItemType.Function,
                            Arguments = args,
                            FunctionName = name,
                        };
                    }
                    else if (index + 1 < items.Count && IsFunction(items[index], items[index + 1]))
                    {
                        actualArg.Add(ParseFunction(items, items[index].Content, index + 2, out index));
                    }
                    else
                    {
                        actualArg.Add(items[index]);
                    }
                }
                throw new Exception("Invalid function call, missing \"(\"");
            }

            private static bool IsFunction(ExpressionItem a, ExpressionItem b)
            {
                if (a.Type == ExpressionItemType.Variable)
                {
                    if (b.Type == ExpressionItemType.OpenBracket)
                    {
                        return true;
                    }
                }
                return false;
            }

            public static string[] SplitExpression(string expression)
            {
                var parts = expressionSplitRegex.Split(expression);

                // Handling edge cases like empty entries
                List<string> result = new List<string>();
                foreach (var part in parts)
                {
                    if (!string.IsNullOrEmpty(part.Trim()))
                        result.Add(part.Trim());
                }

                return result.ToArray();
            }
        }

        public enum ActionType
        {
            ERROR,
            EMPTY,
            If,
            ElseIf,
            Else,
            Fi,
            Assign,
            While,
            End
        }

        public enum ExpressionItemType
        {
            ERROR,
            UnaryOperator,
            Operator,
            Function,
            Value,
            Variable,
            OpenBracket,
            CloseBracket,
            ArgumentDelimeter
        }

        public enum ValueType
        {
            ERROR,
            Boolean,
            Numeric,
            String
        }

        public class ActionItem
        {
            public ActionType ActionType = default;
            public string Content = default;
            public string Variable;
            public string Type;
            public ExpressionItem[] Expression = default;

            public void Report()
            {
                Console.WriteLine(Content);
                Console.WriteLine($"    ActionType: {ActionType}");

                if (Type != null)
                    Console.WriteLine($"    Type:       {Type}");

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

        public ActionItem[] CompileCode(string[] lines)
        {
            var result = new List<ActionItem>();
            foreach (var line in lines)
            {
                var action = CompileLine(line.Trim());
                result.Add(action);
            }
            return result.ToArray();
        }

        private ActionItem CompileLine(string line)
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
                result.Expression = Expression.ParseExpression(match.Groups["if_expression"].Value);
            }
            else if (match.Groups["else"].Success)
            {
                result.ActionType = ActionType.Else;
            }
            else if (match.Groups["fi"].Success)
            {
                result.ActionType = ActionType.Fi;
            }
            else if (match.Groups["assignValue"].Success)
            {
                result.ActionType = ActionType.Assign;
                result.Type = match.Groups["assignValue_type"].Value;
                result.Expression = Expression.ParseExpression(match.Groups["assignValue_expression"].Value);
                result.Variable = match.Groups["assignValue_variable"].Value;
            }
            else if (match.Groups["while"].Success)
            {
                result.ActionType = ActionType.While;
                result.Expression = Expression.ParseExpression(match.Groups["while_expression"].Value);
            }
            else if (match.Groups["end"].Success)
            {
                result.ActionType = ActionType.End;
            }
            else
            {
                result.ActionType = ActionType.ERROR;
            }

            return result;
        }
    }
    //public class Interpreter
    //{
    //    private static class Expression
    //    {
    //        public static readonly Regex expressionSplitRegex;
    //        private static readonly string[] expresionDelimiters = new[]
    //        {
    //            "&&", "||", "<=", ">=", "==", "!=", "<", ">",
    //            "+", "-", "*", "/", "%", "&", "|",
    //            " ", "(", ")"
    //        };

    //        static Expression()
    //        {
    //            string expressionSplitPattern = "(" + String.Join("|", expresionDelimiters.Select(x => Regex.Escape(x))) + ")";
    //            expressionSplitRegex = new Regex(expressionSplitPattern, RegexOptions.Compiled);
    //        }

    //        public static ExpressionItem[] ParseExpression(string expression)
    //        {
    //            var items = Expression.SplitExpression(expression);
    //            List<ExpressionItem> result = new List<ExpressionItem>();

    //            foreach (var item in items)
    //            {
    //                var typematch = Patterns.ExpressionTerminalRegex.Match(item);
    //                var res = new ExpressionItem
    //                {
    //                    Content = item
    //                };

    //                if (!typematch.Success)
    //                    res.Type = ExpressionItemType.ERROR;
    //                else if (typematch.Groups["boolOperator"].Success)
    //                    res.Type = ExpressionItemType.Operator;
    //                else if (typematch.Groups["aritmeticOperator"].Success)
    //                    res.Type = ExpressionItemType.Operator;
    //                else if (typematch.Groups["value"].Success)
    //                {
    //                    res.Type = ExpressionItemType.Value;

    //                    var valueTypeMatch = Patterns.ValueTypeRegex.Match(item);
    //                    if (valueTypeMatch.Groups["booleanValue"].Success)
    //                    {
    //                        res.ValueType = ValueType.Boolean;
    //                        var boolean = bool.Parse(item);
    //                        if(boolean)
    //                        {
    //                            res.Value = 1;
    //                        }
    //                        else
    //                        {
    //                            res.Value = 0;
    //                        }
    //                    }
    //                    else if (valueTypeMatch.Groups["numericValue"].Success)
    //                    {
    //                        res.ValueType = ValueType.Numeric;
    //                        if(float.TryParse(item, NumberStyles.Any, CultureInfo.InvariantCulture, out float value))
    //                        {
    //                            res.Value = value;
    //                        }
    //                        else
    //                        {
    //                            //TODO: Value exception, invalid format.
    //                            res.Value = 0;
    //                        }
    //                    }
    //                    else if (valueTypeMatch.Groups["stringValue"].Success)
    //                    {
    //                        //res.ValueType = ValueType.Boolean;
    //                        //res.StringValue = item.Trim('"');
    //                    }
    //                    else
    //                    {
    //                        res.ValueType = ValueType.ERROR;
    //                    }


    //                }
    //                else if (typematch.Groups["variable"].Success)
    //                    res.Type = ExpressionItemType.Variable;
    //                else if (typematch.Groups["openBracket"].Success)
    //                    res.Type = ExpressionItemType.OpenBracket;
    //                else if (typematch.Groups["closeBracket"].Success)
    //                    res.Type = ExpressionItemType.CloseBracket;

    //                result.Add(res);
    //            }

    //            return result.ToArray();
    //        }

    //        public static string[] SplitExpression(string expression)
    //        {
    //            var parts = expressionSplitRegex.Split(expression);

    //            // Handling edge cases like empty entries
    //            List<string> result = new List<string>();
    //            foreach (var part in parts)
    //            {
    //                if (!string.IsNullOrEmpty(part.Trim()))
    //                    result.Add(part.Trim());
    //            }

    //            return result.ToArray();
    //        }
    //    }

    //    public enum ActionType
    //    {
    //        ERROR,
    //        EMPTY,
    //        If,
    //        ElseIf,
    //        Else,
    //        Fi,
    //        Assign,
    //        While,
    //        End
    //    }

    //    public enum ExpressionItemType
    //    {
    //        ERROR,
    //        UnaryOperator,
    //        Operator,
    //        Value,
    //        Variable,
    //        OpenBracket,
    //        CloseBracket
    //    }

    //    public enum ValueType
    //    {
    //        ERROR,
    //        Boolean,
    //        Numeric,
    //        String
    //    }

    //    public class ActionItem
    //    {
    //        public ActionType ActionType = default!;
    //        public string Content = default!;
    //        public string? Variable;
    //        public string? Type;
    //        public ExpressionItem[]? Expression = default!;

    //        public void Report()
    //        {
    //            Console.WriteLine(Content);
    //            Console.WriteLine($"    ActionType: {ActionType}");

    //            if (Type != null)
    //                Console.WriteLine($"    Type:       {Type}");

    //            if (Variable != null)
    //                Console.WriteLine($"    Variable:   {Variable}");

    //            if (Expression != null)
    //            {
    //                Console.WriteLine($"    Expression:");
    //                foreach (var item in Expression)
    //                {
    //                    Console.WriteLine($"        {item}");
    //                }
    //            }
    //        }
    //    }

    //    public class ExpressionItem
    //    {
    //        public ExpressionItemType Type = default!;
    //        public ValueType ValueType;
    //        public string Content = default!;

    //        public float? Value;
    //        public override string ToString()
    //        {
    //            if (Type == ExpressionItemType.Value)
    //                return $"<{Type}-{ValueType}> {Content}";
    //            else
    //                return $"<{Type}> {Content}";
    //        }
    //    }

    //    public ActionItem[] CompileCode(string[] lines)
    //    {
    //        var result = new List<ActionItem>();
    //        foreach (var line in lines)
    //        {
    //            var action = CompileLine(line.Trim());
    //            result.Add(action);
    //        }
    //        return result.ToArray();
    //    }

    //    private ActionItem CompileLine(string line)
    //    {
    //        var result = new ActionItem();
    //        result.Content = line;

    //        var match = Patterns.LineRegex.Match(line);
    //        if (line.StartsWith("#") || match.Groups["empty"].Success)
    //        {
    //            result.ActionType = ActionType.EMPTY;
    //        }
    //        else if (match.Groups["if"].Success)
    //        {
    //            result.ActionType = ActionType.If;
    //            result.Expression = Expression.ParseExpression(match.Groups["if_expression"].Value);
    //        }
    //        else if (match.Groups["elseif"].Success)
    //        {
    //            result.ActionType = ActionType.ElseIf;
    //            result.Expression = Expression.ParseExpression(match.Groups["if_expression"].Value);
    //        }
    //        else if (match.Groups["else"].Success)
    //        {
    //            result.ActionType = ActionType.Else;
    //        }
    //        else if (match.Groups["fi"].Success)
    //        {
    //            result.ActionType = ActionType.Fi;
    //        }
    //        else if (match.Groups["assignValue"].Success)
    //        {
    //            result.ActionType = ActionType.Assign;
    //            result.Type = match.Groups["assignValue_type"].Value;
    //            result.Expression = Expression.ParseExpression(match.Groups["assignValue_expression"].Value);
    //            result.Variable = match.Groups["assignValue_variable"].Value;
    //        }
    //        else if (match.Groups["while"].Success)
    //        {
    //            result.ActionType = ActionType.While;
    //            result.Expression = Expression.ParseExpression(match.Groups["while_expression"].Value);
    //        }
    //        else if (match.Groups["end"].Success)
    //        {
    //            result.ActionType = ActionType.End;
    //        }
    //        else
    //        {
    //            result.ActionType = ActionType.ERROR;
    //        }

    //        return result;
    //    }
    //}
}

