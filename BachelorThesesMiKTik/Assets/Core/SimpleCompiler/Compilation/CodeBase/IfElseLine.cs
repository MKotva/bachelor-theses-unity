using System.Collections.Generic;
using Assets.Core.SimpleCompiler.Enums;
using Assets.Core.SimpleCompiler.Exceptions;

namespace Assets.Core.SimpleCompiler.Compilation.CodeBase
{
    public class IfElseLine : ICodeLine
    {
        public int LineNumber { get; set; }

        private List<Condition> conditions;
        private Condition elseCondition;

        public IfElseLine(List<Condition> conditions, Condition elseCondition = null)
        {
            this.conditions = conditions;
            this.elseCondition = elseCondition;
        }
        public void Execute()
        {
            foreach (var condition in conditions)
            {
                if (condition.ConditionExpression == null)
                {
                    throw new RuntimeException($"Condition parsing error at line {condition.LineNumber}!");
                }

                if (ExecuteCondition(condition))
                    return;
            }

            if (elseCondition != null)
            {
                foreach (var line in elseCondition.CodeLines)
                {
                    line.Execute();
                }
            }
        }

        private bool ExecuteCondition(Condition condition)
        {
            var cond = condition.ConditionExpression.Evaluate();
            if (cond.Type != ValueType.Boolean) 
            {
                throw new RuntimeException($"Invalid if condition on line {condition.LineNumber}! Expression is type of {cond.Type}");
            }
            if ((bool)cond.Value)
            {
                foreach (var line in condition.CodeLines)
                {
                    line.Execute();
                }
                return true;
            }
            return false;
        }
    }
}
