namespace Assets.Core.SimpleCompiler.Exceptions
{
    public class RuntimeException : CompilerException
    {
        public RuntimeException(string message = "", int line = -1) : base(line, "Runtime error", message) { }
    }
}
