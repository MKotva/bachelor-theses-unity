namespace Assets.Core.SimpleCompiler.Exceptions
{
    public class CompilationException : CompilerException
    {
        public CompilationException(string message = "", int line = -1) : base(line, "Runtime error", message) { }
    }
}
