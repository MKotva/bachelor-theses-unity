using RegexTest.DTOS;
using RegexTest.ExpressionItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RegexTest.CodeBase
{
    public class CodeContext
    {
        public Dictionary<string, Operand> LocalVariables { get; set; }
        private Dictionary<string, (object, Type)> connectedObjects;

        public CodeContext(Dictionary<string, (object, Type)> conectedObjects) 
        {
            LocalVariables = new Dictionary<string, Operand>();
            connectedObjects = conectedObjects;
        }
        public Method GetMethod(string path, int numberOfArguments)
        {
            var pathMembers = SplitPath(path);
            var methodName = pathMembers.Last();
            var objectPath = pathMembers.Take(pathMembers.Count() - 1).ToArray();

            var actingObject = FindObject(objectPath);
            var methodInfo = GetMethod(actingObject.Item2, methodName, numberOfArguments);
            return new Method(methodInfo, actingObject.Item1);
        }

        public  Property GetProperty(string path)
        {
            var pathMembers = SplitPath(path);
            var propName = pathMembers.Last();
            var objectPath = pathMembers.Take(pathMembers.Count() - 1).ToArray();

            var actingObject = FindObject(objectPath);
            var propInfo = GetProperty(actingObject.Item2, propName);
            return new Property(propInfo, actingObject.Item1);
        }

        private (object,Type) FindObject(string[] pathMembers)
        {
            var actualObject = connectedObjects[pathMembers[0]];      
            for (int i = 1; i < pathMembers.Count(); i++)
            {
                var propInfo = GetProperty(actualObject.Item2, pathMembers[1]);
                var value = propInfo.GetValue(actualObject.Item1);
                if (value == null)
                    throw new Exception($"Unassigned property {propInfo.Name}");
                actualObject = (value, propInfo.PropertyType);
            }
            return actualObject;
        }

        private PropertyInfo GetProperty(Type type, string name)
        {
            var properties = type.GetProperties();
            foreach (var property in properties)
            {
                if(property.Name == name)
                {
                    return property; 
                }
            }
            throw new Exception($"Non existring property {name}");
        }

        private List<MethodInfo> GetMethod(Type type, string name, int numberOfArguments)
        {
            var methods = type.GetMethods();

            var selected = new List<MethodInfo>();
            foreach (var method in methods)
            {
                if (method.Name == name && method.GetParameters().Length == numberOfArguments)
                {
                    selected.Add(method);
                }
            }

            if(selected.Count != 0)
                return selected;

            throw new Exception($"Non existring method {name} with {numberOfArguments} arguments.");
        }

        private string[] SplitPath(string path)
        {
            var pathMembers = path.Split('.');
            if (pathMembers.Length < 1)
                throw new Exception($"Non existing object: {path}");
            return pathMembers;
        }
    }
}
