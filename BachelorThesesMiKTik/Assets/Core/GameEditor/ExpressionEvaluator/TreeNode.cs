using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Core.GameEditor.ExpressionEvaluator
{
    public enum NodeType 
    { 
        None, Number, Operation
    }

    public abstract class TreeNode
    {
        NodeType Type { get; set; }
        public abstract float Evaluate();
    }
}
