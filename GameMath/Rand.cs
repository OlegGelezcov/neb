

namespace GameMath
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public static class Rand
    {
        private static Random random = new Random((int)DateTime.Now.Ticks & 0x0000FFFF);

        public static Vector3 RandomBetweenTwoVectors(Vector3 v1, Vector3 v2)
        {
            float x = v1.X + Float01() * (v2.X - v1.X);
            float y = v1.Y + Float01() * (v2.Y - v1.Y);
            float z = v1.Z + Float01() * (v2.Z - v1.Z);
            return new Vector3(x, y, z);
        }

        public static float Float01()
        {
            return (float)random.NextDouble();
        }

        public static float Float(float max)
        {
            return Float01() * max;
        }

        public static float Float(float min, float max)
        {
            return min + (max - min) * Float01();
        }

        public static int Int()
        {
            return random.Next();
        }

        public static int Int(int min, int max)
        {
            return random.Next(min, max + 1);
        }

        public static int[] GenerateNumbers(int numCount, int sum) {
            int[] result = new int[numCount];
            for(int i = 0; i < result.Length; i++) {
                result[i] = 0;
            }

            for(int i = 0; i < sum; i++) {
                result[Int(0, numCount - 1)]++;
            }
            return result;
        }

        public static int Int(int max)
        {
            return Int(0, max);
        }

        public static Vector3 UnitVector3() {
            float x = 2 * Float01() - 1;
            float y = 2 * Float01() - 1;
            float z = 2 * Float01() - 1;
            Vector3 result = new Vector3(x, y, z);
            while (result.Length == 0.0f) {
                x = 2 * Float01() - 1;
                y = 2 * Float01() - 1;
                z = 2 * Float01() - 1;
                result = new Vector3(x, y, z);
            }
            result.Normalize();
            return result;
        }

        public static Vector3 RandomVector3(float maxLen)
        {
            return UnitVector3() * Float(maxLen);
        }

        public static Vector3 Vector3(float fixedLen)
        {
            return UnitVector3() * fixedLen;
        }

        public static float NormalNumber(float left, float right, float rightProb = 0.1f)
        {
            if (rightProb == 0f)
                rightProb = 0.01f;

            float dist = Math.Abs(right - left);
            float sig = (float)(dist / Math.Sqrt(2.0f * Math.Abs(Math.Log(rightProb))));

            float u = Float01();
            float v = Float01();

            float x = (float)Math.Abs(Math.Sqrt(-2.0f * Math.Log(u)) * Math.Cos(2 * Math.PI * v));
            return Mathf.Clamp(left + sig * x, left, right);
        }

        public static float FloatMinMax(float minmax)
        {
            float min = -Mathf.Abs(minmax);
            float max = Mathf.Abs(minmax);
            return Float(min, max);
        }

        public static Vector3 VectorMinMax(float minmax)
        {
            return new Vector3(FloatMinMax(minmax), FloatMinMax(minmax), FloatMinMax(minmax));
        }

        public static int RandomIndex(float[] indexWeights)
        {
            float sum = 0f;
            for (int i = 0; i < indexWeights.Length; i++)
            {
                sum += indexWeights[i];
            }

            for (int i = 0; i < indexWeights.Length; i++)
            {
                indexWeights[i] /= sum;
            }

            float val = Float01();

            float[] borderArray = new float[indexWeights.Length];
            float curSum = 0;
            for (int i = 0; i < borderArray.Length; i++)
            {
                curSum += indexWeights[i];
                borderArray[i] = curSum;
            }

            if (val < borderArray[0])
            {
                return 0;
            }
            for (int i = 1; i < borderArray.Length; i++)
            {
                if (val >= borderArray[i - 1] && val < borderArray[i])
                {
                    return i;
                }
            }
            return borderArray.Length - 1;
        }
    }
}
