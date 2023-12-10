using Assets.Core.GameEditor.DTOS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Core.GameEditor.CodeBase
{
    public class WhileLine : ICodeLine
    {
        private Dictionary<string, float> vars;
        private ConditionNodeDTO loop;

        public WhileLine(Dictionary<string, float> vars, ConditionNodeDTO loop)
        {
            this.vars = vars;
            this.loop = loop;
        }

        public void Execute()
        {
            var cond = loop.Condition.Evaluate();
            while (cond != 0)
            {
                foreach (var line in loop.CodeLines)
                {
                    line.Execute();
                }
            }    
        }
    }
}
