using System.Collections.Generic;
using System.Reflection;
using Assets.Core.SimpleCompiler.Compilation.ExpressionItems;
using Assets.Core.SimpleCompiler.Enums;
using Assets.Core.SimpleCompiler.Exceptions;

namespace Assets.Core.SimpleCompiler.Compilation.CodeBase
{
    public class Method
    {
        public List<MethodInfo> MethodInfo { get; set; }
        public object Instace { get; set; }
        public bool IsReturning { get; set; }

        public Method(List<MethodInfo> methodInfo, object instace)
        {
            MethodInfo = methodInfo;
            Instace = instace;
            if ("Void" != MethodInfo[0].ReturnType.Name)
                IsReturning = true;
        }

        public Operand Invoke(object[] arguments)
        {
            var method = SelectMethod(arguments);
            var result = method.Invoke(Instace, arguments);
            if (IsReturning)
            {
                if (ValueTypeParser.TryGetType(result, out var type))
                {
                    return new Operand(result, type);
                }
            }

            return new Operand(null, ValueType.Empty);
        }

        private MethodInfo SelectMethod(object[] arguments)
        {
            foreach (var methodInfo in MethodInfo)
            {
                var parameters = methodInfo.GetParameters();

                bool argumentMatch = true;
                for(int i = 0; i < arguments.Length; i++)
                {
                    if (parameters[i].ParameterType != arguments[i].GetType())
                    {
                        argumentMatch = false;
                        break;
                    }
                }

                if(argumentMatch)
                { 
                    return methodInfo; 
                }
            }

            throw new RuntimeException($"There is no method {MethodInfo[0].Name} with equivalent arguments");
        }
    }
}
