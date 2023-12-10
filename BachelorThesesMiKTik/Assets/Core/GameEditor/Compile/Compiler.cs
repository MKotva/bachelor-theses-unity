using Assets.Core.GameEditor.CodeBase;
using Assets.Core.GameEditor.DTOS;
using Assets.Core.GameEditor.ExpressionEvaluator;
using Mono.Cecil.Cil;
using System.Collections.Generic;
using System.Linq;
using static Assets.Core.GameEditor.Compile.Interpreter;

namespace Assets.Core.GameEditor.Compile
{
    public class Compiler
    {
        private Dictionary<string, float> variables;
        private Interpreter interpreter;

        public Compiler() 
        {
            variables = new Dictionary<string, float>();
            interpreter = new Interpreter();
        }

        public MyCode CompileCode(string[] textLines)
        {
            variables.Clear();
            var lines = interpreter.CompileCode(textLines);

            var code = new List<ICodeLine>();
            for(int index = 0; index < lines.Length; index++) 
            {
                var codeLine = CompileLine(lines, index, out index);
                if(codeLine != null) 
                {
                    code.Add(codeLine);
                }
            }

            return new MyCode(variables, code);
        }
        
        private ICodeLine CompileLine(ActionItem[] lines, int index, out int endIndex)
        {
            endIndex = index;
            var line = lines[index];

            if (line.ActionType == Interpreter.ActionType.Assign)
            {
                return new AssignLine(variables, ParseExpression(line), line.Variable);
            }
            else if (line.ActionType == Interpreter.ActionType.If)
            {
                return ParseIfElse(lines, index, out endIndex);
            }
            else if(line.ActionType == Interpreter.ActionType.While)
            {
                return ParseWhile(lines, index, out endIndex);
            }

            return null;
        }

        private TreeNode ParseExpression(ActionItem item)
        {
            var expr = new ExpressionTree(item.Expression, variables);
            TreeNode node;
            try
            {
                node = expr.Build();
            }
            catch
            {
                InfoPanelController.print("Exception");
                return null;
            }
            return node;
        }

        private IfElseLine ParseIfElse(ActionItem[] lines, int lineIndex, out int endIndex)
        {
            var conditions = new List<ConditionNodeDTO>();
            var condition = new ConditionNodeDTO(ParseExpression(lines[lineIndex]), new List<ICodeLine>());
            ConditionNodeDTO elseCondition = null;

            for(int index = lineIndex + 1; index < lines.Length; index++)
            {
                var line = lines[index];
                if(line.ActionType == ActionType.ElseIf)
                {
                    conditions.Add(condition);
                    condition = new ConditionNodeDTO(ParseExpression(lines[index]), new List<ICodeLine>());
                    continue;
                }
                else if(line.ActionType == ActionType.Else)
                {
                    conditions.Add(condition);
                    elseCondition = new ConditionNodeDTO(null, new List<ICodeLine>());
                    condition = elseCondition;
                    continue;
                }
                else if (line.ActionType == ActionType.Fi)
                {
                    endIndex = index;
                    return new IfElseLine(variables, conditions, elseCondition);
                }

                condition.CodeLines.Add(CompileLine(lines, index, out endIndex));
                index = endIndex;
            }

            //TODO: Throw missing fi exception.
            endIndex = lines.Count() - 1;
            return null;
        }

        public WhileLine ParseWhile(ActionItem[] lines, int lineIndex, out int endIndex)
        {
            var loop = new ConditionNodeDTO(ParseExpression(lines[lineIndex]), new List<ICodeLine>());
            for (int index = lineIndex + 1; index < lines.Length; index++)
            {
                var line = lines[index];
                if (line.ActionType == ActionType.End)
                {
                    endIndex = lineIndex + 1;
                    return new WhileLine(variables, loop);
                }

                loop.CodeLines.Add(CompileLine(lines, index, out endIndex));
                index = endIndex;
            }

            endIndex = lines.Count();
            return null;
        }
    }
}
