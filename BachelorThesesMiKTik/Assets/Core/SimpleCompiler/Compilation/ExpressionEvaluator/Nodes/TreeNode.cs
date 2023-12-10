﻿using Assets.Core.SimpleCompiler.Compilation.ExpressionItems;

namespace Assets.Core.SimpleCompiler.Compilation.ExpressionEvaluator.Nodes
{
    public enum NodeType
    {
        None, Number, Operation
    }

    public abstract class TreeNode
    {
        public NodeType Type { get; set; }
        public abstract Operand Evaluate();
    }
}
