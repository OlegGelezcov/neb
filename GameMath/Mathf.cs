namespace GameMath
{
    using System;

    public static class Mathf
    {
        public const float DEG2RAD = (float)(Math.PI / 180.0f);
        public const float RAD2DEG = (float)(180.0f / Math.PI);
        public const float FEPS = 0.0001f;

        public static float PI
        {
            get
            {
                return (float)System.Math.PI;
            }
        }

        public static float HALF_PI {
            get {
                return PI * 0.5f;
            }
        }

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

        public static int RoundToInt(float val) {
            return (int)Math.Round(val);
        }

        public static float Abs(float f)
        {
            return (float)Math.Abs(f);
        }

        public static int Abs(int i) {
            return Math.Abs(i);
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

        public static float Pow(float b, float p)
        {
            return (float)Math.Pow(b, p);
        }

        public static float Atan2(float y, float x)
        {
            return (float)Math.Atan2(y, x);
        }

        public static int Clamp(int t, int min, int max)
        {
            int realMin = Math.Min(min, max);
            int realMax = Math.Max(min, max);
            int result = t;
            if (result < realMin)
                result = realMin;
            if (result > realMax)
                result = realMax;
            return result;
        }

        public static float Clamp(float t, float min, float max)
        {
            float result = t;
            float realMin = Math.Min(min, max);
            float realMax = Math.Max(min, max);
            if (result < realMin)
                result = realMin;
            if (result > realMax)
                result = realMax;
            return result;
        }

        public static float Clamp01(float t)
        {
            return Clamp(t, 0f, 1f);
        }

        public static bool Approximately(float a, float b)
        {
            if (Abs(a - b) < FEPS)
                return true;
            else
                return false;
        }

        public static bool NotEqual(float a, float b) {
            return (!Approximately(a, b));
        }

        public static bool IsZero(float f) {
            return Approximately(f, 0f);
        }


        public static float Lerp(float a, float b, float t)
        {
            return a + t * (b - a);
        }

        public static Vector3 Lerp(Vector3 a, Vector3 b, float t)
        {
            return a + t * (b - a);
        }

        

        public static bool CompareFloat(float first, float second)
        {
            if (Abs(first - second) < FEPS)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static float ClampLess(float value, float min) {
            if(value < min) {
                value = min;
            }
            return value;
        }

        public static int ClampLess(int iVal, int iMin) {
            if(iVal < iMin ) {
                iVal = iMin;
            }
            return iVal;
        }
    }
}