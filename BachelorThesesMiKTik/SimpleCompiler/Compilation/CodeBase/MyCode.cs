using System;
using System.Collections.Generic;
namespace RegexTest.CodeBase
{
    public class MyCode : CodeLine
    {
        public int LineNumber { get; set; }
        public List<CodeLine> Lines { get; set; }
        public string Code { get; set;}
        public CodeContext Context { get; set; }

        public MyCode(string code, Dictionary<string, (object, Type)> enviroment)
        {
            Code = code;
            Context = new CodeContext(enviroment);
            Compile();
        }

        public void Execute()
        {
            foreach(var line in Lines)
            {
                try
                {
                    line.Execute();
                }
                catch(Exception e) 
                {
                    Console.WriteLine($"Runtime error at line {line.LineNumber}: {e.Message}!"); //TODO: add line index!
                }
            }
        }

        private void Compile()
        {
            var compiler = new Compiler();
            Lines = compiler.CompileCode(Context, Code.Split(new string[] { Environment.NewLine }, StringSplitOptions.None));
        }
    }
}
