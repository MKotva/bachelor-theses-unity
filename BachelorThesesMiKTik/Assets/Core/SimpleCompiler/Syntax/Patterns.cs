using System.Text;
using System.Text.RegularExpressions;

namespace Assets.Core.SimpleCompiler.Syntax
{
    public static class Patterns
    {
        //// language=regex
        //private static readonly string variable = @"((?<![^\s+*\/\-\(\)])\w+(?![^\s+*\/-]))";
        ////// language=regex
        //private static readonly string function = $"((?<![^\\s+*\\/\\-\\(\\)])\\w+\\s*(\\((\\s*{funcArgument}\\s*,)*(\\s*{funcArgument}\\s*)\\)|\\(\\s*\\)(?![^\\s+*\\/-])))";
        // language=regex
        private static readonly string variable = @"((?<![^\s+*\/\-\(\)])(\w*[a-zA-Z]\w*\.)*(\w*[a-zA-Z]\w*)(?![^\s+*\/-]))";
        //// language=regex
        //private static readonly string variable = @"((\w*[a-zA-Z]\w*\.)*(\w*[a-zA-Z]\w*))";
        // language=regex
        private static readonly string booleanValue = "(true|false)";
        // language=regex
        private static readonly string numericValue = "([0-9]+(?:\\.[0-9]*)?)";
        // language=regex
        private static readonly string stringValue = "(\"[^\"]*\")";
        // language=regex
        private static readonly string value = $"({booleanValue}|{numericValue}|{stringValue})";
        // language=regex
        private static readonly string boolOperator = @"(\!|\<\=|\>\=|\=\=|\!\=|\|\||\&\&|\>|\<)";
        // language=regex
        private static readonly string aritmeticOperator = @"(\+|\-|\*|\/|\%|\||\&|\^|\>\>|\<\<)";
        // language=regex
        private static readonly string type = @"(num|bool)";
        // language=regex
        private static readonly string typeKeyWords = @"(^|\s)(num|bool|string)(\s|$)";
        // language=regex
        private static readonly string keyWords = @"(^|\s)(if|while|elseif|else|fi|end|while)(\s|$)";
        // language=regex
        private static readonly string funcArg = @"\s*(.*\s*,)*(.*\s*)*\s*";
        // language=regex
        private static readonly string func = $"({variable}\\s*\\({funcArg}\\))*";
        // language=regex
        private static readonly string expression = $"(({func}|{variable}|{value}|\\(|\\)|\\ |{boolOperator}|{aritmeticOperator})+(,)*)";
        // language=regex
        private static readonly string ifPattern = $"^if (?<if_expression>{expression})$";
        // language=regex
        private static readonly string elseifPattern = $"^elseif (?<elseif_expression>{expression})$";
        // language=regex
        private static readonly string elsePattern = "^else$";
        // language=regex
        private static readonly string fiPattern = "^fi$";
        // language=regex
        private static readonly string assignLinePattern = $"^\\s*((?<assignValue_type>{type})\\s*)?(?<assignValue_variable>{variable})\\s*=\\s*(?<assignValue_expression>{expression})\\s*$";
        // language=regex
        private static readonly string simpleLinePattern = $"^\\s*(?<assignValue_expression>{expression})\\s*$";
        // language=regex
        private static readonly string whilePattern = $"^while (?<while_expression>{expression})$";
        // language=regex
        private static readonly string endPattern = "^end$";
        // language=regex
        private static readonly string valueTypePattern = $"((?<stringValue>{stringValue})|(?<booleanValue>{booleanValue})|(?<numericValue>{numericValue}))";


        public static readonly Regex LineRegex;
        public static readonly Regex ExpressionTerminalRegex;
        public static readonly Regex ValueTypeRegex;
        public static readonly Regex KeyWordRegex;
        public static readonly Regex TypeKeyWordRegex;
        public static readonly Regex AssingRegex;

        private const RegexOptions regexOptions = RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.Singleline;

        static Patterns()
        {
            StringBuilder linePattern = new StringBuilder();
            linePattern.Append("(");
            linePattern.Append("(?<empty>(^\\s*$))");
            linePattern.Append($"|(?<if>{ifPattern})");
            linePattern.Append($"|(?<elseif>{elseifPattern})");
            linePattern.Append($"|(?<else>{elsePattern})");
            linePattern.Append($"|(?<fi>{fiPattern})");
            linePattern.Append($"|(?<while>{whilePattern})");
            linePattern.Append($"|(?<assignLine>{assignLinePattern})");
            linePattern.Append($"|(?<end>{endPattern})");
            linePattern.Append($"|(?<simpleLine>{simpleLinePattern})");
            linePattern.Append(")");

            StringBuilder expressionTerminalPattern = new StringBuilder();
            expressionTerminalPattern.Append("(");
            expressionTerminalPattern.Append($"(?<boolOperator>{boolOperator})");
            expressionTerminalPattern.Append($"|(?<aritmeticOperator>{aritmeticOperator})");
            expressionTerminalPattern.Append($"|(?<value>{value})");
            expressionTerminalPattern.Append($"|(?<variable>{variable})");
            expressionTerminalPattern.Append($"|(?<openBracket>(\\())");
            expressionTerminalPattern.Append($"|(?<closeBracket>(\\)))");
            expressionTerminalPattern.Append($"|(?<funcArgDelimeter>(,))");
            expressionTerminalPattern.Append(")");


            LineRegex = new Regex(linePattern.ToString(), regexOptions);
            ExpressionTerminalRegex = new Regex(expressionTerminalPattern.ToString(), regexOptions);
            ValueTypeRegex = new Regex(valueTypePattern, regexOptions);
            KeyWordRegex = new Regex(keyWords, regexOptions);
            TypeKeyWordRegex = new Regex(typeKeyWords, regexOptions);
            AssingRegex = new Regex(assignLinePattern, regexOptions);
        }
    }
}
