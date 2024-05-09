using Assets.Core.SimpleCompiler.Compilation.ExpressionItems;
using Assets.Core.SimpleCompiler.Enums;
using Assets.Core.SimpleCompiler.Exceptions;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

using String = System.String;

namespace Assets.Core.SimpleCompiler.Syntax
{
    public static class Expression
    {
        public static readonly Regex expressionSplitRegex;
        public static readonly Regex stringRegexPattern;
        private static readonly string[] expresionDelimiters = new[]
        {
                "(\"[^\"]*\")", "&&", "||", "<=", ">=", "==", "!=", "<", ">",
                "+", "-", "*", "/", "%", "&", "|", "!",
                @"[ ](?=(?:[^""]*""[^""]*"")*[^""]*$)", "(", ")", ",", " "
        };
        private static readonly string stringDelimeter = "(\"[^\"]*\")";

        static Expression()
        {
            //string expressionSplitPattern = "(" + String.Join("|", expresionDelimiters.Select(x => Regex.Escape(x))) + ")";
            //expressionSplitRegex = new Regex(expressionSplitPattern, RegexOptions.Compiled);

            string expressionSplitPattern = "(" + String.Join("|", expresionDelimiters.Select(x => Regex.Escape(x))) + ")";
            expressionSplitRegex = new Regex(expressionSplitPattern, RegexOptions.Compiled);
            stringRegexPattern = new Regex(stringDelimeter, RegexOptions.Compiled);
        }

        /// <summary>
        /// Parses expression in string to its proper expression items (operator, variable, value, function etc.) and 
        /// for values parses to setted value type.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static Item[] ParseExpression(string expression)
        {
            var items = Expression.SplitExpression(expression);
            List<Item> result = new List<Item>();
            bool functionDetected = false;

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
                    if (valueTypeMatch.Groups["stringValue"].Success)
                    {
                        res.ValueType = ValueType.String;
                        res.Value = item.Trim('"');
                    }
                    else if (valueTypeMatch.Groups["booleanValue"].Success)
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
                            throw new SyntaxException($"Error was encountered during float parsing in expression {expression}!");
                        }
                    }
                    else
                    {
                        res.ValueType = ValueType.ERROR;
                    }


                }
                else if (typematch.Groups["variable"].Success)
                    res.Type = ExpressionItemType.Variable;
                else if (typematch.Groups["openBracket"].Success)
                {
                    if (result.Last().Type == ExpressionItemType.Variable)
                        functionDetected = true;

                    res.Type = ExpressionItemType.OpenBracket;
                }
                else if (typematch.Groups["closeBracket"].Success)
                    res.Type = ExpressionItemType.CloseBracket;
                else if (typematch.Groups["funcArgDelimeter"].Success)
                    res.Type = ExpressionItemType.ArgumentDelimeter;
                else
                    res.Type = ExpressionItemType.ERROR;

                result.Add(res);
            }

            if(functionDetected)
                result = FindFunctions(result);
            return result.ToArray();
        }

        /// <summary>
        /// Finds fuction in parsed expression items.
        /// </summary>
        /// <param name="items">expression items</param>
        /// <returns></returns>
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

        /// <summary>
        /// Creates function expression item with his parameters(another expressions)
        /// </summary>
        /// <param name="items"></param>
        /// <param name="name"></param>
        /// <param name="startIndex"></param>
        /// <param name="newIndex"></param>
        /// <returns></returns>
        /// <exception cref="SyntaxException"></exception>
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
                        throw new SyntaxException("Empty function argument");
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
                        throw new SyntaxException("Empty function argument");
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
            throw new SyntaxException("Invalid function call, missing \"(\"");
        }

        /// <summary>
        /// Determines if founded variable is name of function.[variable '(']
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Splits expression based on expression pattern.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        private static string[] SplitExpression(string expression)
        {
            var parts = GetParts(expression);

            // Handling edge cases like empty entries
            List<string> result = new List<string>();
            foreach (var part in parts)
            {
                if (!string.IsNullOrEmpty(part.Trim()))
                    result.Add(part.Trim());
            }

            return result.ToArray();
        }

        private static string[] GetParts(string input)
        {
            var stringMatches = stringRegexPattern.Split(input);

            var res = new List<string>();
            foreach (var stringMatch in stringMatches)
            {
                if (stringRegexPattern.Match(stringMatch).Success)
                    res.Add(stringMatch);
                else
                {
                    var results = expressionSplitRegex.Split(stringMatch);
                    foreach (var result in results)
                        res.Add(result);
                }
            }
            return res.ToArray();
        }
    }
}
