namespace Assets.Core.SimpleCompiler.Exceptions
{
    public class SyntaxException : CompilerException
    {
        public SyntaxException(string message = "", int line = -1) : base(line, "Runtime error", message) { }
    }
}
