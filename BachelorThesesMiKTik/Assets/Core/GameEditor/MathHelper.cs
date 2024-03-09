using System;
using UnityEngine;

namespace Assets.Core.GameEditor
{
    public static class MathHelper
    {
        public static Vector3 GetVector3FromString(string stringVector)
        {
            var values = stringVector.Split(':');
            if (values.Length == 2)
                return new Vector3((float) Convert.ToDouble(values[0]), (float) Convert.ToDouble(values[1]));
            else if (values.Length == 3)
                return new Vector3((float) Convert.ToDouble(values[0]), (float) Convert.ToDouble(values[1]), (float) Convert.ToDouble(values[2]));

            return new Vector3();
        }

        public static Vector3 Add(Vector3 a, Vector2 b)
        {
            a.x += b.x;
            a.y += b.y;
            return a;
        }

        public static float Pow(float x, int exp) 
        {
            var powered = x;

            for(int i = 1; i < exp; i++)
                powered *= powered;

            return powered;
        }
        public static float GetFloat(string text, string name = "", string author = "")
        {
            var value = 1f;
            if (!float.TryParse(text, out value))
            {
                InfoPanelController.Instance.ShowMessage($"{name} parsing error! {name} was setted to 1", author);
                return 1;
            }
            if(float.IsNaN(value))
            {
                InfoPanelController.Instance.ShowMessage($"{name} parsing error! {name} was setted to 1", author);
                return 1;
            }

            return value;
        }
        public static float GetPositiveFloat(string text, string name = "", string author = "")
        {
            var value = 1f;
            if (!float.TryParse(text, out value))
            {
                InfoPanelController.Instance.ShowMessage($"{name} parsing error! {name} was setted to 1", author);
                return 1f;
            }
            if (value < 0f || float.IsNaN(value))
            {
                InfoPanelController.Instance.ShowMessage($"{name} parsing error, value is below 0! {name} was setted to 1", author);
                return 1f;
            }
            return value;
        }

        public static uint GetUInt(string input, uint defaultValue = 0, string name = "", string author = "")
        {
            if (uint.TryParse(input, out uint value))
            {
                return value;
            }
            else
            {
                InfoPanelController.Instance.ShowMessage($"{name} parsing error! {name} was setted to {defaultValue}", author);
                return defaultValue;
            }
        }
    }
}
