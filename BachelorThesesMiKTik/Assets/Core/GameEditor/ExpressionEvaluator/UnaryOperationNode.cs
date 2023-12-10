using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Core.GameEditor.ExpressionEvaluator
{
    public class UnaryOperationNode : TreeNode
    {
        private Operator op;
        private TreeNode operand;

        public UnaryOperationNode(Operator operation, TreeNode operand)
        {
            this.op = operation;
            this.operand = operand;
        }

        public override float Evaluate()
        {
            switch(op.OperatorText) 
            {
                case "!": return AritmeticNegation(operand);
                case "-": return BooleanNegation(operand);
                default : return 0; //TODO: Invalid operation exception
            }
        }

        private float AritmeticNegation(TreeNode operand)
        {
            return -1 * operand.Evaluate(); 
        }

        private float BooleanNegation(TreeNode operand) 
        {
            var value = operand.Evaluate();
            if (value == 0)
                return 1;

            return 0;
        }
    }
}
