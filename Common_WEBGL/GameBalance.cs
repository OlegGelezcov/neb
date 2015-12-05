namespace Common {
    using GameMath;
    using System;

    public static class GameBalance {
        public static float ComputeHitProb(float optimalDistance, float targetDistance, float size) {

            if (size > 1f) {
                optimalDistance += size;
            }

            float maxProb = 1f;

            if (targetDistance <= optimalDistance) {
                return maxProb;
            }

            float p = maxProb - (0.9f / optimalDistance) * (targetDistance - optimalDistance);
            if (p < 0.01f) {
                p = 0f;
            }
            return p;
        }


        public static float StandardNormalRandomNumber() {
            float u = Rand.Float01();
            float v = Rand.Float01();
            float x = Mathf.Sqrt(-2f * Mathf.Log(u)) * Mathf.Cos(2f * Mathf.PI * v);
            return x;
        }
    }
}
