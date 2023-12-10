using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Core.GameEditor.CodeBase
{
    public class MyCode : ICodeLine
    {
        public Dictionary<string, float> Variables { get; set; }
        public List<ICodeLine> Lines { get; set; }

        public MyCode(Dictionary<string, float> variables, List<ICodeLine> lines) 
        {
            Variables = variables;
            Lines = lines;
        }

        public void Execute()
        {
            foreach (var line in Lines) 
            {
                line.Execute();
            }
        }
    }
}
