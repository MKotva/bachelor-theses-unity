using Assets.Core.GameEditor.Attributes;
using Assets.Core.SimpleCompiler.Exceptions;
using UnityEngine;

namespace Assets.Core.GameEditor.CodeEditor.EnviromentObjects
{
    public class Converters : EnviromentObject
    {
        public override bool SetInstance(GameObject instance) { return true; }

        [CodeEditorAttribute("Converts given num (toConvert) to string.", "returns string, ( num toConvert)")]
        public static string NumToString(float f) 
        {
            return f.ToString();
        }

        [CodeEditorAttribute("Converts given num (toConvert) to bool.", "returns bool, ( num toConvert)")]
        public static bool NumToBool(float f)
        {
            if(f ==  0f) 
                return false;
            return true;
        }

        [CodeEditorAttribute("Converts given bool (toConvert) to string.", "returns string, ( bool toConvert)")]
        public static string BoolToString(bool b)
        {
            return b.ToString();
        }

        [CodeEditorAttribute("Converts given bool (toConvert) to num (true = 1, false = 0).", "returns num, ( bool toConvert)")]
        public static float BoolToNum(bool b)
        {
            if (b)
                return 1;
            else
                return 0;
        }

        [CodeEditorAttribute("Converts given string (toConvert) to num(Number must be in correct format).", "returns num, ( string toConvert)")]
        public static float StringToNum(string s)
        {
            float f = 0;
            if(MathHelper.GetFloat(s, out f))
            {
                return f;
            }
            throw new RuntimeException($"Unable to convert string {s} to float!");
        }

        [CodeEditorAttribute("Converts given string (toConvert) to bool(Bool must be in correct format(true|false)).", "returns bool, ( string toConvert)")]
        public static bool StringToBool(string s)
        {
            bool b = false;
            if(bool.TryParse(s, out b))
            {
                return b;
            }

            throw new RuntimeException($"Unable to convert string {s} to bool!");
        }
    }
}
