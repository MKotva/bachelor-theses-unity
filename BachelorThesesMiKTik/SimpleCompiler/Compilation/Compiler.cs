using System;
using System.Collections.Generic;
using System.Linq;
using RegexTest.CodeBase;
using RegexTest.DTOS;
using RegexTest.Enums;
using RegexTest.ExpressionEvaluator;
using RegexTest.Semantic;

namespace RegexTest
{
    public class Compiler
    {
        private CodeContext context;
        private SemanticAnalyzer interpreter;

        public Compiler()
        {
            interpreter = new SemanticAnalyzer();
        }

        public List<CodeLine> CompileCode(CodeContext codeContext, string[] textLines)
        {
            context = codeContext;
            var lines = interpreter.AnalyzeCode(textLines);

            var code = new List<CodeLine>();
            for (int index = 0; index < lines.Length; index++)
            {
                CodeLine codeLine;
                try
                {
                    codeLine = CompileLine(lines, index, out index);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Compilation error on line {index} : {ex.Message}");
                }

                if (codeLine != null)
                {
                    code.Add(codeLine);
                    codeLine.LineNumber = index;
                }
            }
            return code;
        }

        private CodeLine CompileLine(ActionItem[] lines, int index, out int endIndex)
        {
            endIndex = index;
            var line = lines[index];

            switch(line.ActionType)
            {
                case ActionType.Assign : return new AssignLine(context, ParseExpression(line), line.Variable, line.VariableType);
                case ActionType.SimpleLine : return new SimpleLine(context, ParseExpression(line));
                case ActionType.If : return ParseIfElse(lines, index, out endIndex);
                case ActionType.While : return ParseWhile(lines, index, out endIndex);
                case ActionType.ERROR: throw new Exception($"Compile error, invalid action syntax!");
                default: return null;           
            }
        }

        private TreeNode ParseExpression(ActionItem item)
        {
            var expr = new ExpressionTree(item.Expression, context);
            TreeNode node;
            try
            {
                node = expr.Build();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
            return node;
        }

        private IfElseLine ParseIfElse(ActionItem[] lines, int lineIndex, out int endIndex)
        {
            var conditions = new List<Condition>();
            var condition = new Condition(ParseExpression(lines[lineIndex]), new List<CodeLine>(), lineIndex);
            Condition elseCondition = null;

            for (int index = lineIndex + 1; index < lines.Length; index++)
            {
                var line = lines[index];
                if (line.ActionType == ActionType.ElseIf)
                {
                    conditions.Add(condition);
                    condition = new Condition(ParseExpression(lines[index]), new List<CodeLine>(), index);
                }
                else if (line.ActionType == ActionType.Else)
                {
                    conditions.Add(condition);
                    elseCondition = ParseElse(lines, index, out endIndex);
                    return new IfElseLine(conditions, elseCondition);
                }
                else if (line.ActionType == ActionType.Fi)
                {
                    conditions.Add(condition);
                    endIndex = index;
                    return new IfElseLine(conditions, elseCondition);
                }
                else
                {
                    condition.CodeLines.Add(CompileLine(lines, index, out var end));
                    index = end;
                }
            }

            throw new Exception("If is missing ending \"fi\" statement.");
        }

        private Condition ParseElse(ActionItem[] lines, int lineIndex, out int endIndex)
        {
            var elseCode = new List<CodeLine>();
            for (int index = lineIndex + 1; index < lines.Length; index++)
            {
                var line = lines[index];
                if (line.ActionType == ActionType.ElseIf || line.ActionType == ActionType.Else)
                {
                    throw new Exception($"Invalid statement {line.ActionType.ToString()} in \"else\" condition. Add if or restucturalize.");
                }

                if (line.ActionType == ActionType.Fi)
                {
                    endIndex = index;
                    return new Condition(null, elseCode, index);
                }

                elseCode.Add(CompileLine(lines, index, out var end));
                index = end;
            }
            throw new Exception("If is missing ending \"fi\" statement.");
        }

        private  WhileLine ParseWhile(ActionItem[] lines, int lineIndex, out int endIndex)
        {
            var loop = new Condition(ParseExpression(lines[lineIndex]), new List<CodeLine>(), lineIndex);
            for (int index = lineIndex + 1; index < lines.Length; index++)
            {
                var line = lines[index];
                if (line.ActionType == ActionType.End)
                {
                    endIndex = index;
                    return new WhileLine(loop);
                }

                loop.CodeLines.Add(CompileLine(lines, index, out var end));
                index = end;
            }

            throw new Exception("The while is missing \"end\" statement.");
        }
    }
}
