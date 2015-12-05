using System;


namespace Common {
    public static class Geometry {
        public static SphericalCoord Cartesian2Spherical(CVec vec) {

            float r = (vec.IsZero == false) ? (float)Math.Sqrt(vec.x * vec.x + vec.y * vec.y + vec.z * vec.z) : 0.0f;
            float theta = (vec.z != 0.0f) ? (float)Math.Atan2(Math.Sqrt(vec.x * vec.x + vec.y * vec.y), vec.z) : 0.0f;
            float phi = (vec.x != 0.0f) ? (float)Math.Atan2(vec.y, vec.x) : 0.0f;
            if (phi < 0)
                phi += (float)(2.0f * Math.PI);
            return new SphericalCoord(r, theta, phi);
        }

        public static CVec Spherical2Cartesian(SphericalCoord sc) {
            float x = (float)(sc.R * Math.Sin(sc.Thteta) * Math.Cos(sc.Phi));
            float y = (float)(sc.R * Math.Sin(sc.Thteta) * Math.Sin(sc.Phi));
            float z = (float)Math.Cos(sc.Thteta);
            return new CVec(x, y, z);
        }
    }
}
