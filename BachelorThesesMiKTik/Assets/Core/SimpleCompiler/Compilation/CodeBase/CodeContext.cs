using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Assets.Core.GameEditor.Attributes;
using Assets.Core.GameEditor.CodeEditor.EnviromentHandlers;
using Assets.Core.GameEditor.DTOS;
using Assets.Core.GameEditor.DTOS.CodeEditor;
using Assets.Core.SimpleCompiler.Compilation.ExpressionItems;
using Assets.Core.SimpleCompiler.Exceptions;
using ValueType = Assets.Core.SimpleCompiler.Enums.ValueType;

namespace Assets.Core.SimpleCompiler.Compilation.CodeBase
{
    public class CodeContext
    {
        public Dictionary<string, Operand> LocalVariables { get; private set; }
        public Dictionary<string, Operand> GlobalVariables { get; private set; }
        public Dictionary<string, EnviromentContextDTO> EnviromentObjects{ get; private set; }

        public CodeContext(List<EnviromentObjectDTO> enviromentObjects, List<GlobalVariableDTO> globalVariables) 
        {
            LocalVariables = new Dictionary<string, Operand>();
            EnviromentObjects = GetEnviromentContext(enviromentObjects);
            GlobalVariables = GetGlobalVariableContext(globalVariables);
            if(!CheckNaming(EnviromentObjects, GlobalVariables, out var name))
            {
                throw new CompilationException($"There are enviroment and global variable with same name {name}!");
            }
        }
        /// <summary>
        /// After finding the "Acting object" (with FindActingObject()) the method will
        /// search his type for methods with the same name as last member of the path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns>Methods info and instances in Method class</returns>
        public Method GetMethod(string path, int numberOfArguments)
        {
            var pathMembers = SplitPath(path);
            var methodName = pathMembers.Last();
            var objectPath = pathMembers.Take(pathMembers.Count() - 1).ToArray();

            var actingObject = FindActingObject(objectPath);
            var methodInfo = GetMethod(actingObject.Type, methodName, numberOfArguments);
            return new Method(methodInfo, actingObject.Instance);
        }

        /// <summary>
        /// After finding the "Acting object" (with FindActingObject()) the method will
        /// search his type for property with the same name as last member of the path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns>Property info and instance in Property class</returns>
        public  Property GetProperty(string path)
        {
            var pathMembers = SplitPath(path);
            var propName = pathMembers.Last();
            var objectPath = pathMembers.Take(pathMembers.Count() - 1).ToArray();

            var actingObject = FindActingObject(objectPath);
            var propInfo = GetProperty(actingObject.Type, propName);
            return new Property(propInfo, actingObject.Instance);
        }

        /// <summary>
        /// Method goes through the splitted path (by SplitPath()) and finds 
        /// the "Acting object" on which type the property or the method will
        /// be searched.
        /// 
        /// If any member of the path can not be found, method will rise CompilationEx.
        /// </summary>
        /// <param name="pathMembers"></param>
        /// <returns></returns>
        /// <exception cref="CompilationException"></exception>
        private EnviromentContextDTO FindActingObject(string[] pathMembers)
        {
            if (!EnviromentObjects.ContainsKey(pathMembers[0]))
                throw new CompilationException($"Unknown enviroment object with name \"{pathMembers[0]}\"");

            var actualObject = EnviromentObjects[pathMembers[0]];      
            for (int i = 1; i < pathMembers.Count(); i++)
            {
                var propInfo = GetProperty(actualObject.Type, pathMembers[1]);
                var value = propInfo.GetValue(actualObject.Instance);
                if (value == null)
                    throw new CompilationException($"Unassigned property {propInfo.Name}");
                actualObject = new EnviromentContextDTO(value, propInfo.PropertyType);
            }
            return actualObject;
        }

        /// <summary>
        /// Checks the type and finds property which is marked with CodeEditorAttribute and 
        /// which has the same name. 
        ///
        /// If no such property are found, compilation error is raised.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="CompilationException"></exception>
        private PropertyInfo GetProperty(Type type, string name)
        {
            var properties = type.GetProperties();
            foreach (var property in properties)
            {
                var attributes = property.GetCustomAttributes<CodeEditorAttribute>();
                if(property.Name == name && attributes.Count() != 0)
                {
                    return property; 
                }
            }
            throw new CompilationException($"Non existring property {name}");
        }

        /// <summary>
        /// Checks the type and finds methods which is marked with CodeEditorAttribute and 
        /// which has the same name, argument lenght. 
        ///
        /// If no such methods are found, compilation error is raised.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="numberOfArguments"></param>
        /// <returns></returns>
        /// <exception cref="CompilationException"></exception>
        private List<MethodInfo> GetMethod(Type type, string name, int numberOfArguments)
        {
            var methods = type.GetMethods();

            var selected = new List<MethodInfo>();
            foreach (var method in methods)
            {
                var attributes = method.GetCustomAttributes<CodeEditorAttribute>();
                if (method.Name == name && method.GetParameters().Length == numberOfArguments && attributes.Count() != 0)
                {
                    selected.Add(method);
                }
            }

            if(selected.Count != 0)
                return selected;

            throw new CompilationException($"Non existring method {name} with {numberOfArguments} arguments.");
        }

        /// <summary>
        /// Splits given path (e.g. name.name.metho()/ name.property) by "."
        /// If path has less than 2 members, throws compilation error, because
        /// there is no object with path shorter than 2.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="CompilationException"></exception>
        private string[] SplitPath(string path)
        {
            var pathMembers = path.Split('.');
            if (pathMembers.Length < 2)
                throw new CompilationException($"Non existing object: {path}");
            return pathMembers;
        }

        private Dictionary<string, EnviromentContextDTO> GetEnviromentContext(List<EnviromentObjectDTO> enviromentDTOS)
        {
            var enviromentObjects = new Dictionary<string, EnviromentContextDTO>();
            foreach(var env in  enviromentDTOS)
            {
                if (EnviromentController.TryGetInstance(env.TypeName, out object instace))
                    enviromentObjects.Add(env.Alias, new EnviromentContextDTO(instace, instace.GetType()));
            }
            return enviromentObjects;
        }

        private Dictionary<string, Operand> GetGlobalVariableContext(List<GlobalVariableDTO> globalVarDTOS)
        {
            var globalVariables = new Dictionary<string, Operand>();
            foreach(var globalVar in globalVarDTOS)
            {
                var operand = new Operand(ParseValue(globalVar.Type, globalVar.Value), globalVar.Type);
                globalVariables.Add(globalVar.Alias, operand);
            }
            return globalVariables;
        }

        /// <summary>
        /// Checks if enviroment and global variables dont have same variable name
        /// </summary>
        /// <param name="enviroment"></param>
        /// <param name="global"></param>
        /// <param name="sameName"></param>
        /// <returns></returns>
        private bool CheckNaming(Dictionary<string, EnviromentContextDTO> enviroment, Dictionary<string, Operand> global, out string sameName)
        {
            foreach (var key in enviroment.Keys)
            {
                if (global.ContainsKey(key))
                {
                    sameName = key;
                    return false;
                }
            }
            sameName = "";
            return true;
        }


        /// <summary>
        /// Parses input string to proper type based on selected value type an encapsulates it to object.
        /// If input string is empty, creates default object of given type.
        /// </summary>
        /// <param name="valueType"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="CompilationException"></exception>
        private object ParseValue(ValueType valueType, string input)
        {
            if (input == "")
                return GetDefaultValue(valueType);

            if (valueType == ValueType.Boolean)
            {
                if (bool.TryParse(input, out bool result))
                    return result;
                throw new CompilationException($"Invalid global variable default value {input}");
            }
            else if (valueType == ValueType.Numeric)
            {
                if (float.TryParse(input, out float result))
                    return result;
                throw new CompilationException($"Invalid global variable default value {input} for value type {valueType}");
            }

            return input;
        }

        private object GetDefaultValue(ValueType valueType)
        {
            switch (valueType)
            {
                case ValueType.String: return string.Empty;
                case ValueType.Boolean: return false;
                default: return 0f;
            }
        }
    }
}
