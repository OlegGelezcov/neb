using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameMath
{
    #region ARRAY UTILS
    public static class ArrayUtils
    {
        public const float EPS = 1E-6f;

        public static void Multiply(ref float[] vector, float value)
        {
            for (int i = 0; i < vector.Length; i++)
            {
                vector[i] *= value;
            }
        }

        public static float[] Multiply(float[] vector, float value)
        {
            float[] result = new float[3];
            for (int i = 0; i < 3; i++)
            {
                result[i] = vector[i] * value;
            }
            return result;
        }

        public static void Add(ref float[] first, float[] second)
        {
            for (int i = 0; i < first.Length; i++)
            {
                first[i] += second[i];
            }
        }

        public static float Distance(float[] first, float[] second)
        {
            float dx = second[0] - first[0];
            float dy = second[1] - first[1];
            float dz = second[2] - first[2];
            return (float)Math.Sqrt(dx * dx + dy * dy + dz * dz);
        }

        public static float DistanceSq(float[] first, float[] second)
        {
            float dx = second[0] - first[0];
            float dy = second[1] - first[1];
            float dz = second[2] - first[2];
            return dx * dx + dy * dy + dz * dz;
        }

        public static float Length(float[] vector)
        {
            return (float)Math.Sqrt(vector[0] * vector[0] + vector[1] * vector[1] + vector[2] * vector[2]);
        }

        public static void Normalize(ref float[] vector)
        {
            float len = Length(vector);
            vector[0] /= len;
            vector[1] /= len;
            vector[2] /= len;
        }

        public static float[] GetDirection(float[] from, float[] to)
        {
            float[] direction = new float[3];
            direction[0] = to[0] - from[0];
            direction[1] = to[1] - from[1];
            direction[2] = to[2] - from[2];
            if (!AlmostEqual(direction, new float[] { 0.0f, 0.0f, 0.0f }))
            {
                Normalize(ref direction);
            }
            else
            {
                direction[0] = Rand.Float(-1, 1);
                direction[1] = Rand.Float(-1, 1);
                direction[2] = Rand.Float(-1, 1);
                Normalize(ref direction);
            }

            return direction;
        }

        public static bool AlmostEqual(float[] first, float[] second)
        {
            if (!Mathf.Approximately(first[0], second[0]))
                return false;
            if (!Mathf.Approximately(first[1], second[1]))
                return false;
            if (!Mathf.Approximately(first[2], second[2]))
                return false;
            return true;
        }

        public static string GetString(float[] vector)
        {
            return string.Format("({0:F2}, {1:F2}, {2:F2})", vector[0], vector[1], vector[2]);
        }


    }
    #endregion
}
