using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Core.SimpleCompiler.Compilation.ExpressionItems
{
    public enum OperatorType
    {
        None,
        Bracket,
        Unary,
        Binary,
        Assign
    }

    public class Operator
    {
        /// <summary>
        /// All valid unary operators sorted in priority groups. 
        /// </summary>
        private static string[][] UnaryOperatorGroups =
        {
            new string [] {"!", "-"}
        };

        /// <summary>
        /// All valid binary operators sorted in priority groups. 
        /// </summary>
        private static string[][] BinaryOperatorGroups =
        {
            new string[] {"*", "/", "%"},
            new string[] {"+", "-"},
            new string[] {"<<", ">>"},
            new string[] {"<", "<=", "=>", ">"},
            new string[] {"==", "!="},
            new string[] {"&"},
            new string[] {"^"},
            new string[] {"|"},
            new string[] {"&&"},
            new string[] {"||"},
        };

        public OperatorType OperatorType { get; private set; }
        public string OperatorText { get; set; }
        public int Priority { get; private set; }

        public Operator(string op, OperatorType operatorType)
        {
            OperatorText = op;
            OperatorType = operatorType;

            if (OperatorType == OperatorType.Unary)
            {
                Priority = GetUnaryPriority();
            }
            else if (OperatorType == OperatorType.Binary)
            {
                Priority = GetBinaryPriority();
            }
            else if(OperatorType == OperatorType.Assign) 
            {
                Priority = BinaryOperatorGroups.Count();
            }
        }

        /// <summary>
        /// Finds unary operator priority based on position in array[position][].
        /// </summary>
        /// <returns>Operator priority</returns>
        private int GetUnaryPriority()
        {
            for (int i = 0; i < UnaryOperatorGroups.Length; i++)
            {
                if (UnaryOperatorGroups[i].Contains(OperatorText))
                {
                    return i - UnaryOperatorGroups.Length;
                }
            }
            return 0;
        }

        /// <summary>
        /// Finds binary operator priority based on position in array[position][].
        /// </summary>
        /// <returns>Operator priority</returns>
        private int GetBinaryPriority()
        {
            for (int i = 0; i < BinaryOperatorGroups.Length; i++)
            {
                if (BinaryOperatorGroups[i].Contains(OperatorText))
                {
                    return i + 1;
                }
            }
            return 0;
        }
    }
}
