using System;
using System.Collections.Generic;
using Assets.Core.GameEditor.CodeEditor.EnviromentObjects;
using Assets.Core.GameEditor.DTOS;
using Assets.Core.SimpleCompiler.Compilation;
using Assets.Core.SimpleCompiler.Compilation.CodeBase;
using Assets.Core.SimpleCompiler.Exceptions;
using UnityEngine;

namespace Assets.Core.SimpleCompiler
{
    [Serializable]
    public class SimpleCode
    {
        /// <summary>
        /// Plain text code
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Code text output
        /// </summary>
        public string Output { get; set; } = "";

        /// <summary>
        /// Code error output
        /// </summary>
        public string ErrorOutput { get; set; } = "";

        /// <summary>
        /// Sets maximum string capacity for output
        /// </summary>
        public uint OutputCapacity { get; private set; } = 1000000;

        /// <summary>
        /// List of connected references to provided API.
        /// </summary>
        public List<EnviromentObjectDTO> EnviromentObjects { get; private set; }

        /// <summary>
        /// List of global variables created in code editor connected to code.
        /// </summary>
        public List<GlobalVariableDTO> GlobalVariables { get; private set; }

        /// <summary>
        /// Context for code, such as connected objects, global variables etc.
        /// </summary>
        private CodeContext Context { get; set; }

        /// <summary>
        /// Compiled code
        /// </summary>
        private CodeBlock Main { get; set; }


        public SimpleCode(string code, List<EnviromentObjectDTO> enviroment, List<GlobalVariableDTO> globalVariables)
        {
            Code = code;
            EnviromentObjects = enviroment;
            GlobalVariables = globalVariables;
            Context = new CodeContext(enviroment, globalVariables);
            Compile();
        }

        /// <summary>
        /// Compiles given test to SimpleCode.
        /// </summary>
        private void Compile()
        {
            try
            {
                if(Context == null) 
                {
                    LoadContext();
                }
                var compiler = new Compiler();
                Main = new CodeBlock(compiler.CompileCode(Context, Code.Split(new string[] { Environment.NewLine, "\n" }, StringSplitOptions.None)), 0);
            }
            catch (CompilerException e)
            {
                ErrorOutput = $"Error in section Main:\n {e.Message}!";
                ErrorOutputManager.Instance.ShowMessage(Output);
            }
        }

        /// <summary>
        /// If code is compiled, than this method will execute it. At the same time, listener for console output
        /// is added for runtime.
        /// </summary>
        public void Execute(GameObject instance)
        {
            ErrorOutputManager.Instance.AddOnAddListener("Compiler", ConsoleHandler, "Console");
            if (Main == null)
            {
                Compile();
            }
            else
            {
                try
                {
                    SetDependecies(instance);
                    Context.LocalVariables.Clear();
                    Main.Execute();
                }
                catch (CompilerException e)
                {
                    ErrorOutput += $"Error in section Main:\n {e.Message}!";
                    ErrorOutputManager.Instance.ShowMessage(Output);
                }
            }
            ErrorOutputManager.Instance.RemoveListener("Compiler");
        }

        //TODO: Remove after finished
        public void TestExecute()
        {
            ErrorOutputManager.Instance.AddOnAddListener("Compiler", ConsoleHandler, "Console");
            if (Main == null)
            {
                ErrorOutput += "Code cant be executed because was not compiled!";
            }
            else
            {
                try
                {
                    Context.LocalVariables.Clear();
                    Main.Execute();
                }
                catch (CompilerException e)
                {
                    ErrorOutput += $"Error in section Main:\n {e.Message}!";
                    ErrorOutputManager.Instance.ShowMessage(Output);
                }
            }
            ErrorOutputManager.Instance.RemoveListener("Compiler");
        }

        /// <summary>
        /// Handles output of the console.
        /// </summary>
        /// <param name="message"></param>
        private void ConsoleHandler(string message)
        {
            if (Output.Length > OutputCapacity)
                Output = Output.Substring(Output.Length / 2);
            Output += $"{message}\n";
        }

        private void SetDependecies(GameObject instance)
        {
            foreach(var dependency in Context.EnviromentObjects)
            {
                var depInstance = dependency.Value.Instance;
                if (depInstance is EnviromentObject)
                {
                    var enviroment = (EnviromentObject)depInstance;
                    enviroment.SetInstance(instance);
                }
            }
        }

        private void LoadContext()
        {
            if (EnviromentObjects != null && GlobalVariables != null) 
            {
                Context = new CodeContext(EnviromentObjects, GlobalVariables);
            }
            throw new CompilationException("Context loading exception! Enviroment objects or Global variables missing!");
        }
    }
}

