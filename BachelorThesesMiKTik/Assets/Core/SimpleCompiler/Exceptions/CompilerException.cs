using System;

namespace Assets.Core.SimpleCompiler.Exceptions
{
    public class CompilerException : Exception
    {
        public CompilerException(int line, string exceptionType, string message) : base(CreateMessage(line, exceptionType, message)) { }

        private static protected string CreateMessage(int line, string exceptionType, string message)
        {
            if (line > 0)
                return $"{exceptionType} on line \"{line}\" : \"{message}\"";
            else
                return $"{exceptionType} on line \"UNKNOWN\" : \"{message}\"";
        }
    }
}
