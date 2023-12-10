using System;
using System.Collections.Generic;
using Assets.Core.SimpleCompiler.Compilation;
using Assets.Core.SimpleCompiler.Compilation.CodeBase;

namespace Assets.Core.SimpleCompiler
{
    public class SimpleCode
    {
        public CodeBlock Main { get; set; }
        public string Code { get; set; }
        public CodeContext Context { get; set; }


        public SimpleCode(string code, Dictionary<string, (object, Type)> enviroment)
        {
            Code = code;
            Context = new CodeContext(enviroment);
            Compile();
        }
        private void Compile()
        {
            var compiler = new Compiler();
            Main = new CodeBlock(compiler.CompileCode(Context, Code.Split(new string[] { Environment.NewLine }, StringSplitOptions.None)), 0); //TODO: Mockup, prepared for function definition
        }

        public void Execute()
        {
            try
            { 
                Main.Execute(); 
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error in section {Main.LineNumber}: {e.Message}!"); //TODO: add line index!
            }

        }
    }
}

