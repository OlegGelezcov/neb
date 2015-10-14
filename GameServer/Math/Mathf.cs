namespace Space
{
    using System;

    public static class Mathf
    {
        public const float DEG2RAD = (float)(Math.PI / 180.0f);
        public const float RAD2DEG = (float)(180.0f / Math.PI);

        public static float Sin(float f)
        {
            return (float)Math.Sin(f);
        }

        public static float Cos(float f)
        {
            return (float)Math.Cos(f);
        }

        public static float Acos(float f)
        {
            return (float)Math.Acos(f);
        }

        public static float Sqrt(float f)
        {
            return (float)Math.Sqrt(f);
        }

        public static float Abs(float f)
        {
            return (float)Math.Abs(f);
        }

        public static float Log(float f)
        {
            return (float)Math.Log(f);
        }

        public static float Exp(float f)
        {
            return (float)Math.Exp(f);
        }

        public static float Asin(float f)
        {
            return (float)Math.Asin(f);
        }

        public static float Atan2(float y, float x)
        {
            return (float)Math.Atan2(y, x);
        }

        public static float Clamp(float value, float min, float max)
        {
            if (value < min)
                value = min;
            if (value > max)
                value = max;
            return value;
        }

        public static float Clamp01(float value)
        {
            return Clamp(value, 0, 1);
        }

        public static bool Approximately(float a, float b)
        {
            float eps = 1e-7f;
            if (Abs(a - b) < eps)
                return true;
            else
                return false;
        }
    }
}