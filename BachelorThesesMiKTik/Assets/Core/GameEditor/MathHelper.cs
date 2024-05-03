using Assets.Core.GameEditor.DTOS.Action;
using System;
using System.Collections.Generic;
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

        public static Vector3 Multiply(Vector3 a, Vector3 b)
        {
            var cx = a.x * b.x;
            var cy = a.y * b.y;
            var cz = a.z * b.z;
            return new Vector3(cx, cy, cz);
        }

        public static float Pow(float x, int exp) 
        {
            var powered = x;

            for(int i = 1; i < exp; i++)
                powered *= powered;

            return powered;
        }
        public static float GetFloat(string text, float defaultVal)
        {
            text.Replace('.', ',');
            var value = 1f;
            if (!float.TryParse(text, out value))
            {
                return defaultVal;
            }
            if (float.IsNaN(value))
            {
                return defaultVal;
            }

            return value;
        }

        public static float GetFloat(string value, float defaultValue = 1f, string name = "", string author = "")
        {
            if(value == "")
                return defaultValue;

            value.Replace(',', '.');

            var result = defaultValue;
            if (!float.TryParse(value, out result))
            {
                OutputManager.Instance.ShowMessage($"{name} parsing error! {name} was setted to 1", author);
                return 1;
            }
            if(float.IsNaN(result))
            {
                OutputManager.Instance.ShowMessage($"{name} parsing error! {name} was setted to 1", author);
                return 1;
            }

            return result;
        }

        public static float GetPositiveFloat(string value, float defaultValue = 1f, string name = "", string author = "")
        {
            if (value == "")
                return defaultValue;

            value.Replace('.', ',');

            var result = defaultValue;
            if (!float.TryParse(value, out result))
            {
                OutputManager.Instance.ShowMessage($"{name} parsing error! {name} was setted to 1", author);
                return defaultValue;
            }
            if (result < 0f || float.IsNaN(result))
            {
                OutputManager.Instance.ShowMessage($"{name} parsing error, value is below 0! {name} was setted to 1", author);
                return defaultValue;
            }
            return result;
        }

        public static uint GetUInt(string input, uint defaultValue = 0, string name = "", string author = "")
        {
            if (uint.TryParse(input, out uint value))
            {
                return value;
            }
            else
            {
                OutputManager.Instance.ShowMessage($"{name} parsing error! {name} was setted to {defaultValue}", author);
                return defaultValue;
            }
        }

        //Obtained from : https://www.geeksforgeeks.org/check-if-two-given-line-segments-intersect/
        #region LineIntersetion
        // The main function that returns true if line segment 'p1q1' 
        // and 'p2q2' intersect. 
        public static bool CheckLineIntersection(Vector2 p1, Vector2 q1, Vector2 p2, Vector2 q2)
        {
            // Find the four orientations needed for general and 
            // special cases 
            int o1 = GetOrientation(p1, q1, p2);
            int o2 = GetOrientation(p1, q1, q2);
            int o3 = GetOrientation(p2, q2, p1);
            int o4 = GetOrientation(p2, q2, q1);

            // General case 
            if (o1 != o2 && o3 != o4)
                return true;

            // Special Cases 
            // p1, q1 and p2 are collinear and p2 lies on segment p1q1 
            if (o1 == 0 && CheckIfLiesOnSegment(p1, p2, q1)) return true;

            // p1, q1 and q2 are collinear and q2 lies on segment p1q1 
            if (o2 == 0 && CheckIfLiesOnSegment(p1, q2, q1)) return true;

            // p2, q2 and p1 are collinear and p1 lies on segment p2q2 
            if (o3 == 0 && CheckIfLiesOnSegment(p2, p1, q2)) return true;

            // p2, q2 and q1 are collinear and q1 lies on segment p2q2 
            if (o4 == 0 && CheckIfLiesOnSegment(p2, q1, q2)) return true;

            return false; // Doesn't fall in any of the above cases 
        }
        // Given three collinear points p, q, r, the function checks if 
        // point q lies on line segment 'pr' 
        private static bool CheckIfLiesOnSegment(Vector2 p, Vector2 q, Vector2 r)
        {
            if (q.x <= Math.Max(p.x, r.x) && q.x >= Math.Min(p.x, r.x) &&
                q.y <= Math.Max(p.y, r.y) && q.y >= Math.Min(p.y, r.y))
                return true;

            return false;
        }

        // To find orientation of ordered triplet (p, q, r). 
        // The function returns following values 
        // 0 --> p, q and r are collinear 
        // 1 --> Clockwise 
        // 2 --> Counterclockwise 
        private static int GetOrientation(Vector2 p, Vector2 q, Vector2 r)
        {
            // See https://www.geeksforgeeks.org/orientation-3-ordered-points/ 
            // for details of below formula. 
            float val = ( q.y - p.y ) * ( r.x - q.x ) -
                    ( q.x - p.x ) * ( r.y - q.y );

            if (val == 0) return 0; // collinear 

            return ( val > 0 ) ? 1 : 2; // clock or counterclock wise 
        }
        #endregion
    }
}
