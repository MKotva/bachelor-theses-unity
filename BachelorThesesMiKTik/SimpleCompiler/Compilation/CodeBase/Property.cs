using RegexTest.Enums;
using RegexTest.ExpressionItems;
using System;
using System.Reflection;

namespace RegexTest.DTOS
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
        private Operand ParseProperty(object value)
        {
            if (Value.TryGetType(value, out var type))
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
