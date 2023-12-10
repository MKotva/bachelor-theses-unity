using System.Linq;

namespace RegexTest.Enums
{
    public static class Value
    {
        public enum VType
        {
            ERROR,
            Empty,
            Boolean,
            Numeric,
            String
        }

        private static string[] Shortcuts = new string[]
        {
            "Error",
            "null",
            "bool",
            "num",
            "string"
        };

        public static bool TryGetType(object value, out VType Type)
        {
            Type = VType.ERROR;
            if (value is float || value is int || value is long || value is double) 
            {
                Type = VType.Numeric;
                return true;
            }
            else if(value is bool)
            {
                Type = VType.Boolean;
                return true; 
            }
            else if(value is string) 
            {
                Type = VType.String;
                return true;
            }
            return false;
        }

        public static VType GetFromName(string name) 
        {
            for(int index = 0; index < Shortcuts.Length; index++) 
            {
                if (Shortcuts[index] == name)
                    return (VType)index;
            }
            return VType.ERROR;
        }
    }
}
