using System;
using System.Collections.Generic;

namespace Assets.Core.SimpleCompiler.Compilation.CodeBase
{
    public class CodeBlock : ICodeLine
    {
        public List<ICodeLine> Lines { get; set; }
        public int LineNumber { get; set; }

        public CodeBlock(List<ICodeLine> lines, int blockLineNumber)
        {
            Lines = lines;
            LineNumber = blockLineNumber;
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
