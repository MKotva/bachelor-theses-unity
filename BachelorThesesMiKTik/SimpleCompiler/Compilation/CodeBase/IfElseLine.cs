using RegexTest.DTOS;
using RegexTest.Enums;
using System;
using System.Collections.Generic;
namespace RegexTest.CodeBase
{
    public class IfElseLine : CodeLine
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
                    throw new Exception($"Condition parsing error at line {condition.LineNumber}!");
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
            if (cond.Type != Value.VType.Boolean) 
            {
                throw new Exception($"Invalid if condition on line {condition.LineNumber}! Expression is type of {cond.Type}");
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
