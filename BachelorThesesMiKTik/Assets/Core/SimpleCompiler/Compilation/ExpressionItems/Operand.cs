﻿using Assets.Core.SimpleCompiler.Enums;

namespace Assets.Core.SimpleCompiler.Compilation.ExpressionItems
{
    public class Operand
    {
        public object Value { get; set; }
        public ValueType Type { get; set; }
        public string Name { get; set; }

        public Operand(object value, ValueType type, string name = "")
        {
            Value = value;
            Type = type;
            Name = name;
        }
    }
}
