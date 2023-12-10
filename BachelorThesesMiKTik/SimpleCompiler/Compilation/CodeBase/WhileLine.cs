using RegexTest.DTOS;

namespace RegexTest.CodeBase
{
    public class WhileLine : CodeLine
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
