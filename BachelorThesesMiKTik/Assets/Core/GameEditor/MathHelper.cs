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
    }
}
