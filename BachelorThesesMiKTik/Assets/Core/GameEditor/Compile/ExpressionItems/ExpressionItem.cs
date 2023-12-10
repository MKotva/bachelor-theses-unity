using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Assets.Core.GameEditor.Compile.Interpreter;
using ValueType = Assets.Core.GameEditor.Compile.Interpreter.ValueType;

namespace Assets.Core.GameEditor.Compile.ExpressionItems
{
    public class ExpressionItem
    {
        public ExpressionItemType Type = default;
        public ValueType ValueType;
        public string Content = default;

        public float? Value;
        public override string ToString()
        {
            if (Type == ExpressionItemType.Value)
                return $"<{Type}-{ValueType}> {Content}";
            else
                return $"<{Type}> {Content}";
        }
    }
}
