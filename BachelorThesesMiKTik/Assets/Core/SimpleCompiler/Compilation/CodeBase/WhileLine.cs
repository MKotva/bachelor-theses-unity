namespace Assets.Core.SimpleCompiler.Compilation.CodeBase
{
    public class WhileLine : ICodeLine
    {
        public int LineNumber { get; set; }

        private Condition loop;

        public WhileLine(Condition loop)
        {
            this.loop = loop;
        }

        public void Execute()
        {
            while (loop.Execute())
            {
                foreach (var line in loop.CodeLines)
                {
                    line.Execute();
                }
            }
        }
    }
}
