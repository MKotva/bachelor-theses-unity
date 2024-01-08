using Assets.Core.SimpleCompiler.Exceptions;

namespace Assets.Core.SimpleCompiler.Compilation.CodeBase
{
    public class WhileLine : ICodeLine
    {
        public int LineNumber { get; set; }

        private Condition loop;
        private int MaxLoopCounter;

        public WhileLine(Condition loop)
        {
            this.loop = loop;
            MaxLoopCounter = 10000;
        }

        public void Execute()
        {
            long loopCounter = 0;
            while (loop.Execute())
            {
                if(loopCounter >= MaxLoopCounter)
                {
                    throw new RuntimeException($"Infinite loop detected on line number {LineNumber}! While exceeded {MaxLoopCounter} iterations!");
                }
                foreach (var line in loop.CodeLines)
                {
                    line.Execute();
                }

                loopCounter++;
            }
        }
    }
}
