﻿using Assets.Core.SimpleCompiler.Enums;
using Assets.Core.SimpleCompiler.Compilation.ExpressionItems;

namespace Assets.Core.SimpleCompiler.Syntax
{
    public class ActionItem
    {
        public ActionType ActionType = default;
        public string Content = default;
        public string Variable;
        public ValueType VariableType;
        public Item[] Expression = default;
        public string AsignOperator = default;
    }
}
