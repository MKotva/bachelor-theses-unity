namespace Assets.Core.SimpleCompiler.Compilation.CodeBase
{
    public interface ICodeLine
    {
        int LineNumber { get; set; }
        void Execute();
    }
}
