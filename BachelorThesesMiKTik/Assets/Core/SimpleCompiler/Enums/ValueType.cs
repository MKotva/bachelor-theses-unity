namespace Assets.Core.SimpleCompiler.Enums
{
    public enum ValueType
    {
        ERROR,
        Empty,
        Boolean,
        Numeric,
        String
    }

    public static class ValueTypeParser
    {
        private static string[] Shortcuts = new string[]
        {
            "Error",
            "null",
            "bool",
            "num",
            "string"
        };

        public static bool TryGetType(object value, out ValueType Type)
        {
            Type = ValueType.ERROR;
            if (value is float || value is int || value is long || value is double) 
            {
                Type = ValueType.Numeric;
                return true;
            }
            else if(value is bool)
            {
                Type = ValueType.Boolean;
                return true; 
            }
            else if(value is string) 
            {
                Type = ValueType.String;
                return true;
            }
            return false;
        }

        public static ValueType GetFromName(string name) 
        {
            for(int index = 0; index < Shortcuts.Length; index++) 
            {
                if (Shortcuts[index] == name)
                    return (ValueType)index;
            }
            return ValueType.ERROR;
        }
    }
}
