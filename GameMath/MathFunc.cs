using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameMath
{
    public class MathFunc
    {
        public static int Clamp(int t, int min, int max)
        {
            return Mathf.Clamp(t, min, max);
        }

        public static float Clamp(float t, float min, float max)
        {
            return Mathf.Clamp(t, min, max);
        }

        public static float Clamp01(float t)
        {
            return Mathf.Clamp01(t);
        }
    }
}
