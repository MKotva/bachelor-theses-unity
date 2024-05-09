using System.Collections.Generic;
using System.Threading.Tasks;
using Assets.Core.SimpleCompiler.Compilation.CodeBase;
using Assets.Core.SimpleCompiler.Compilation.ExpressionEvaluator;
using Assets.Core.SimpleCompiler.Compilation.ExpressionEvaluator.Nodes;
using Assets.Core.SimpleCompiler.Enums;
using Assets.Core.SimpleCompiler.Exceptions;
using Assets.Core.SimpleCompiler.Syntax;

namespace Assets.Core.SimpleCompiler.Compilation
{
    public class Compiler
    {
        private CodeContext context;
        private SemanticAnalyzer interpreter;
        private string exceptions;

        public Compiler()
        {
            interpreter = new SemanticAnalyzer();
        }


        /// <summary>
        /// Runs syntax analyze on code and the based on results will try to build
        /// code segments composed of expression trees from assign and simple lines (compilation).
        /// </summary>
        /// <param name="codeContext"></param>
        /// <param name="textLines"></param>
        /// <returns></returns>
        /// <exception cref="CompilationException"> If there was any error during the compilation</exception>
        public async Task<List<ICodeLine>> CompileCodeAsync(CodeContext codeContext, string[] textLines)
        {
            context = codeContext;
            var lines = await interpreter.AnalyzeCodeAsync(textLines);

            var code = new List<ICodeLine>();
            for (int index = 0; index < lines.Length; index++)
            {
                ICodeLine codeLine;
                try
                {
                    codeLine = CompileLine(lines, index, out index);
                }
                catch (CompilerException ex)
                {
                    exceptions += $"Compilation error on line {index} : {ex.Message}\n";
                    continue;
                }

                if (codeLine != null)
                {
                    code.Add(codeLine);
                    codeLine.LineNumber = index;
                }
            }

            if (exceptions != null && exceptions != "")
                throw new CompilationException(exceptions);

            return code;
        }

        /// <summary>
        /// Runs syntax analyze on code and the based on results will try to build
        /// code segments composed of expression trees from assign and simple lines (compilation).
        /// </summary>
        /// <param name="codeContext"></param>
        /// <param name="textLines"></param>
        /// <returns></returns>
        /// <exception cref="CompilationException"> If there was any error during the compilation</exception>
        public List<ICodeLine> CompileCode(CodeContext codeContext, string[] textLines)
        {
            context = codeContext;
            var lines = interpreter.AnalyzeCode(textLines);

            var code = new List<ICodeLine>();
            for (int index = 0; index < lines.Length; index++)
            {
                ICodeLine codeLine;
                try
                {
                    codeLine = CompileLine(lines, index, out index);
                }
                catch (CompilerException ex)
                {
                    exceptions += $"Compilation error on line {index} : {ex.Message}\n";
                    continue;
                }

                if (codeLine != null)
                {
                    code.Add(codeLine);
                    codeLine.LineNumber = index;
                }
            }

            if (exceptions != null && exceptions != "")
                throw new CompilationException(exceptions);

            return code;
        }

        /// <summary>
        /// Based on syntax analyze, this method will try to create proper code 
        /// block (Assign, SimpleLine, If, Else, While).
        /// </summary>
        /// <param name="lines">Syntax analyze output</param>
        /// <param name="index">Index to analyze output to be parsed</param>
        /// <param name="endIndex">Index on where compilation ended</param>
        /// <returns></returns>
        /// <exception cref="SyntaxException">For uknown code block</exception>
        private ICodeLine CompileLine(ActionItem[] lines, int index, out int endIndex)
        {
            endIndex = index;
            var line = lines[index];

            switch(line.ActionType)
            {
                case ActionType.Assign : return new AssignLine(context, ParseExpression(line), line.Variable, line.VariableType, line.AsignOperator);
                case ActionType.SimpleLine : return new SimpleLine(context, ParseExpression(line));
                case ActionType.If : return ParseIfElse(lines, index, out endIndex);
                case ActionType.While : return ParseWhile(lines, index, out endIndex);
                case ActionType.ERROR: throw new SyntaxException($"Compile error, invalid action syntax!");
                default: throw new SyntaxException($"Compile error, unknown action syntax!");           
            }
        }

        /// <summary>
        /// Builds expression tree from syntax analyzed line.
        /// </summary>
        /// <param name="item">analyzed line</param>
        /// <returns></returns>
        private TreeNode ParseExpression(ActionItem item)
        {
            var expr = new ExpressionTree(item.Expression, context);
            return expr.Build();
        }

        /// <summary>
        /// Parses if/elseif/else code blocks and its conditions to one parent code block.
        /// This parent code block will than on runtime evaluate conditions and run proper (if/elseif/else) code block.
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="lineIndex"></param>
        /// <param name="endIndex"></param>
        /// <returns></returns>
        /// <exception cref="SyntaxException">If if block lacks ending fi statement.</exception>
        private IfElseLine ParseIfElse(ActionItem[] lines, int lineIndex, out int endIndex)
        {
            var conditions = new List<Condition>();
            var condition = new Condition(ParseExpression(lines[lineIndex]), new List<ICodeLine>(), lineIndex);
            Condition elseCondition = null;

            for (int index = lineIndex + 1; index < lines.Length; index++)
            {
                var line = lines[index];
                if (line.ActionType == ActionType.ElseIf)
                {
                    conditions.Add(condition);
                    condition = new Condition(ParseExpression(lines[index]), new List<ICodeLine>(), index);
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

            throw new SyntaxException("If is missing ending \"fi\" statement.");
        }

        /// <summary>
        /// Parses else code block and checks if after else block is fi statement and no other.
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="lineIndex"></param>
        /// <param name="endIndex"></param>
        /// <returns></returns>
        /// <exception cref="SyntaxException"></exception>
        private Condition ParseElse(ActionItem[] lines, int lineIndex, out int endIndex)
        {
            var elseCode = new List<ICodeLine>();
            for (int index = lineIndex + 1; index < lines.Length; index++)
            {
                var line = lines[index];
                if (line.ActionType == ActionType.ElseIf || line.ActionType == ActionType.Else)
                {
                    throw new SyntaxException($"Invalid statement {line.ActionType.ToString()} in \"else\" condition. Add if or restucturalize.");
                }

                if (line.ActionType == ActionType.Fi)
                {
                    endIndex = index;
                    return new Condition(null, elseCode, index);
                }

                elseCode.Add(CompileLine(lines, index, out var end));
                index = end;
            }
            throw new SyntaxException("If is missing ending \"fi\" statement.");
        }

        /// <summary>
        /// Parses while code block, its condition and execute linces to one parent code block.
        /// This parent code block will than on runtime evaluate condition and run execute lines until condition holds.
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="lineIndex"></param>
        /// <param name="endIndex"></param>
        /// <returns></returns>
        /// <exception cref="SyntaxException">Exception for missing end statement.</exception>
        private WhileLine ParseWhile(ActionItem[] lines, int lineIndex, out int endIndex)
        {
            var loop = new Condition(ParseExpression(lines[lineIndex]), new List<ICodeLine>(), lineIndex);
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

            throw new SyntaxException("The while is missing \"end\" statement.");
        }
    }
}
