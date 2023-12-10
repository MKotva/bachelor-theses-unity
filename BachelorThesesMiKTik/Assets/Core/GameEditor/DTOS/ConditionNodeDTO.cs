using Assets.Core.GameEditor.CodeBase;
using Assets.Core.GameEditor.ExpressionEvaluator;
using System.Collections.Generic;

namespace Assets.Core.GameEditor.DTOS
{
    public class ConditionNodeDTO
    {
        public TreeNode Condition { get; set; }
        public List<ICodeLine> CodeLines { get; set; }

        public ConditionNodeDTO(TreeNode condition, List<ICodeLine> code)
        {
            Condition = condition;
            CodeLines = code;
        }
    }
}
