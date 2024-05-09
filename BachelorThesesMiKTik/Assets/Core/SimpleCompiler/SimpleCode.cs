using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Assets.Core.GameEditor.CodeEditor.EnviromentObjects;
using Assets.Core.GameEditor.DTOS;
using Assets.Core.SimpleCompiler.Compilation;
using Assets.Core.SimpleCompiler.Compilation.CodeBase;
using Assets.Core.SimpleCompiler.Exceptions;
using Newtonsoft.Json;
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
        /// List of connected references to provided API.
        /// </summary>
        public List<EnviromentObjectDTO> EnviromentObjects { get; private set; }

        /// <summary>
        /// List of global variables created in code editor connected to code.
        /// </summary>
        public List<GlobalVariableDTO> GlobalVariables { get; private set; }

        /// <summary>
        /// Code text output
        /// </summary>
        [JsonIgnore]
        public string Output { get; set; } = "";

        /// <summary>
        /// Code error output
        /// </summary>
        [JsonIgnore]
        public string ErrorOutput { get; set; } = "";

        /// <summary>
        /// Sets maximum string capacity for output
        /// </summary>
        [JsonIgnore]
        public uint OutputCapacity { get; private set; } = 1000000;

        /// <summary>
        /// Context for code, such as connected objects, global variables etc.
        /// </summary>
        [JsonIgnore]
        private CodeContext Context;

        /// <summary>
        /// Compiled code
        /// </summary>
        [JsonIgnore]
        private CodeBlock Main { get; set; }

        public SimpleCode(string code, List<EnviromentObjectDTO> enviroment, List<GlobalVariableDTO> globalVariables)
        {
            Code = code;
            EnviromentObjects = enviroment;
            GlobalVariables = globalVariables;
        }

        public SimpleCode(SimpleCode code)
        {
            Code = code.Code;
            EnviromentObjects = code.EnviromentObjects;
            GlobalVariables = code.GlobalVariables;
            var task = CompileAsync();
        }

        [JsonConstructor]
        private SimpleCode()
        {
            Code = "";
            EnviromentObjects = new List<EnviromentObjectDTO>();
            GlobalVariables = new List<GlobalVariableDTO>();
        }

        /// <summary>
        /// Compiles given test to SimpleCode async.
        /// </summary>
        public async Task CompileAsync()
        {
            try
            {
                if (Context == null)
                {
                    LoadContext();
                }
                var compiler = new Compiler();
                var lines = await compiler.CompileCodeAsync(Context, Code.Split(new string[] { Environment.NewLine, "\n" }, StringSplitOptions.None));
                Main = new CodeBlock(lines, 0);
            }
            catch (CompilerException e)
            {
                ErrorOutput = $"Error in section Main:\n {e.Message}!";
                OutputManager.Instance.ShowMessage(Output);
            }
        }

        /// <summary>
        /// Compiles given test to SimpleCode.
        /// </summary>
        public void Compile()
        {
            try
            {
                if (Context == null)
                {
                    LoadContext();
                }
                var compiler = new Compiler();
                var lines = compiler.CompileCode(Context, Code.Split(new string[] { Environment.NewLine, "\n" }, StringSplitOptions.None));
                Main = new CodeBlock(lines, 0);
            }
            catch (CompilerException e)
            {
                ErrorOutput = $"Error in section Main:\n {e.Message}!";
                OutputManager.Instance.ShowMessage(e.Message);
            }
        }


        /// <summary>
        /// If code is compiled, than this method will execute it. At the same time, listener for console output
        /// is added for runtime.
        /// </summary>
        public void Execute(GameObject instance)
        {
            OutputManager.Instance.AddOnAddListener("Compiler", ConsoleHandler, "Console");
            if (Main == null)
            {
                ErrorOutput += $"There is no executable code!";
                return; 
            }
            else
            {
                try
                {
                    SetDependecies(instance);
                    Context.LocalVariables.Clear();
                    Main.Execute();
                }
                catch (RuntimeException e)
                {
                    ErrorOutput += $"Error in section Main:\n {e.Message}!";
                    OutputManager.Instance.ShowMessage(e.Message);
                }

                catch (CompilerException e)
                {
                    ErrorOutput += $"Error in section Main:\n {e.Message}!";
                    OutputManager.Instance.ShowMessage(e.Message);
                }

                catch (Exception e)
                {
                    if (e.InnerException is RuntimeException)
                    {
                        ErrorOutput += $"Error in section Main:\n {e.InnerException.Message}!";
                        OutputManager.Instance.ShowMessage(e.InnerException.Message);
                    }
                    else
                    {
                        ErrorOutput += $"Error in section Main:\n {e.Message}!";
                        OutputManager.Instance.ShowMessage(e.Message);
                    }
                }
            }
            OutputManager.Instance.RemoveListener("Compiler");
        }

        /// <summary>
        /// Refreshes actual code context to initial state.
        /// </summary>
        public void ResetContext()
        {
            if(Context != null)
            {
                Context.ResetGlobalVariables(GlobalVariables);
            }
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

        /// <summary>
        /// Sets GameObject instance to dependency class
        /// </summary>
        /// <param name="instance">Instance of gameobject</param>
        private void SetDependecies(GameObject instance)
        {
            if(instance == null)
                throw new RuntimeException("Fatal error, instance is empty!");

            foreach (var dependency in Context.EnviromentObjects)
            {
                var depInstance = dependency.Value.Instance;
                if (depInstance is EnviromentObject)
                {
                    var enviroment = (EnviromentObject) depInstance;
                    if (!enviroment.SetInstance(instance))
                        throw new RuntimeException("Failed to set dependecy! Object does no contain required components!");
                }
            }
        }

        /// <summary>
        /// Creates code context
        /// </summary>
        /// <exception cref="CompilationException"></exception>
        private void LoadContext()
        {
            if (EnviromentObjects != null && GlobalVariables != null)
            {
                Context = new CodeContext(EnviromentObjects, GlobalVariables);
            }
            else
            {
                throw new CompilationException("Context loading exception! Enviroment objects or Global variables missing!");
            }
        }
    }
}

