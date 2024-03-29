﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegexTest.ExpressionEvaluator
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
        private static string[][] UnaryOperatorGroups =
        {
            new string [] {"!", "-"}
        };
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
