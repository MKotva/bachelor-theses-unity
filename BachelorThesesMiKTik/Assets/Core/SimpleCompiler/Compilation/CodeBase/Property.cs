using System;
using System.Reflection;
using Assets.Core.SimpleCompiler.Compilation.ExpressionItems;
using Assets.Core.SimpleCompiler.Enums;

namespace Assets.Core.SimpleCompiler.Compilation.CodeBase
{
    public class Property
    {
        public PropertyInfo PropertyInfo { get; set; }
        public object Instace { get; set; }

        public Property(PropertyInfo propertyInfo, object instace)
        {
            PropertyInfo = propertyInfo;
            Instace = instace;
        }

        public Operand Get()
        {
            return ParseProperty(PropertyInfo.GetValue(Instace));
        }

        public void Set(Operand value)
        {
            PropertyInfo.SetValue(Instace, value.Value);
        }

        /// <summary>
        /// Method will try to parse property output value type to Compilator  value type.
        /// If parse fails, Runtime exception is raised.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private Operand ParseProperty(object value)
        {
            if (ValueTypeParser.TryGetType(value, out var type))
            {
                return new Operand(value, type);
            }
            else
            {
                throw new Exception($"Invalid type of variable {PropertyInfo.Name}");
            }
        }
    }
}
