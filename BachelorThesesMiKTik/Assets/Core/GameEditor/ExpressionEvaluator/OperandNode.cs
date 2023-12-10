using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Core.GameEditor.ExpressionEvaluator
{
    public class OperandNode : TreeNode
    {
        float Value { get; set; }

        public OperandNode(float value)
        {
            Value = value;
        }

        public override float Evaluate()
        {
            return Value;
        }
    }
}
