using Assets.Core.GameEditor.DTOS;
using System;
using System.Collections.Generic;

namespace Assets.Core.GameEditor.CodeBase
{
    public class IfElseLine : ICodeLine
    {
        private Dictionary<string, float> variables;
        private List<ConditionNodeDTO> conditions;
        private ConditionNodeDTO elseCondition;

        public IfElseLine(Dictionary<string, float> variables, List<ConditionNodeDTO> conditions, ConditionNodeDTO elseCondition = null) 
        {
            this.variables = variables;
            this.conditions = conditions;
            this.elseCondition = elseCondition;
        }
        public void Execute()
        {
            foreach (var condition in conditions) 
            {
                if(condition.Condition == null) 
                {
                    //TODO: Throw exception.
                    return;
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

        private bool ExecuteCondition(ConditionNodeDTO condition) 
        {
            var cond = condition.Condition.Evaluate();
            if (cond != 0)
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
