using RegexTest.Enums;
using RegexTest.ExpressionItems;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using ValueType = RegexTest.Enums.Value.VType;

namespace RegexTest.Semantic
{
    public static class Expression
    {
        public static readonly Regex expressionSplitRegex;
        private static readonly string[] expresionDelimiters = new[]
        {
                "&&", "||", "<=", ">=", "==", "!=", "<", ">",
                "+", "-", "*", "/", "%", "&", "|",
                @"[ ](?=(?:[^""]*""[^""]*"")*[^""]*$)", "(", ")", ","
            };

        static Expression()
        {
            string expressionSplitPattern = "(" + String.Join("|", expresionDelimiters.Select(x => Regex.Escape(x))) + ")";
            expressionSplitRegex = new Regex(expressionSplitPattern, RegexOptions.Compiled);
        }

        public static Item[] ParseExpression(string expression)
        {
            var items = Expression.SplitExpression(expression);
            List<Item> result = new List<Item>();

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
                        res.Value = bool.Parse(item);

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
                        res.ValueType = ValueType.String;
                        res.Value = item.Trim('"');
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

        private static List<Item> FindFunctions(List<Item> items)
        {
            var outItems = new List<Item>();

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

        private static MethodItem ParseFunction(List<Item> items, string name, int startIndex, out int newIndex)
        {
            var actualArg = new List<Item>();
            var args = new List<List<Item>> { };
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
                        actualArg = new List<Item>();
                    }
                }
                else if (items[index].Type == ExpressionItemType.CloseBracket)
                {
                    if (args.Count != 0 && actualArg.Count == 0)
                    {
                        throw new Exception("Empty function argument");
                    }
                    else if (actualArg.Count != 0)
                    {
                        args.Add(actualArg);
                    }
                    newIndex = index;
                    return new MethodItem
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

        private static bool IsFunction(Item a, Item b)
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

        private static string[] SplitExpression(string expression)
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
}
