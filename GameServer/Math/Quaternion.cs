namespace Space
{
    using System;

    public struct Quaternion
    {
        private float w;
        private float x;
        private float y;
        private float z;

        public float this[int i]
        {
            get
            {
                switch (i)
                {
                    case 0:
                        return w;
                    case 1:
                        return x;
                    case 2:
                        return y;
                    case 3:
                        return z;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (i)
                {
                    case 0:
                        w = value;
                        break;
                    case 1:
                        x = value;
                        break;
                    case 2:
                        y = value;
                        break;
                    case 3:
                        z = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }


        public float W
        {
            get
            {
                return w;
            }
            set
            {
                w = value;
            }
        }

        public float X
        {
            get
            {
                return x;
            }
            set
            {
                x = value;
            }
        }

        public float Y
        {
            get
            {
                return y;
            }
            set
            {
                y = value;
            }
        }

        public float Z
        {
            get
            {
                return z;
            }
            set
            {
                z = value;
            }
        }

        public Quaternion(float _w, float _x, float _y, float _z)
        {
            w = _w;
            x = _x;
            y = _y;
            z = _z;
        }

        public Quaternion(Quaternion q)
        {
            w = q.w;
            x = q.x;
            y = q.y;
            z = q.z;
        }

        public Quaternion(Vector3 axis, float angle)
            : this(Quaternion.FromAxisAngle(axis, angle))
        {

        }

        public static Quaternion operator +(Quaternion left, Quaternion right)
        {
            return new Quaternion(left.w + right.w, left.x + right.x, left.y + right.y, left.z + right.z);
        }

        public static Quaternion operator -(Quaternion left, Quaternion right)
        {
            return new Quaternion(left.w - right.w, left.x - right.x, left.y - right.y, left.z - right.z);
        }

        public static Quaternion operator *(Quaternion lq, Quaternion rq)
        {
            float w = lq.w * rq.w - lq.x * rq.x - lq.y * rq.y - lq.z * rq.z;
            float x = lq.w * rq.x + lq.x * rq.w + lq.y * rq.z - lq.z * rq.y;
            float y = lq.w * rq.y + lq.y * rq.w + lq.z * rq.x - lq.x * rq.z;
            float z = lq.w * rq.z + lq.z * rq.w + lq.x * rq.y - lq.y * rq.x;
            return new Quaternion(w, x, y, z);
        }

        public static Quaternion operator *(Quaternion q, float s)
        {
            return new Quaternion(q.w * s, q.x * s, q.y * s, q.z * s);
        }

        public static Quaternion operator *(float s, Quaternion q)
        {
            return new Quaternion(q.w * s, q.x * s, q.y * s, q.z * s);
        }

        public static Quaternion operator /(Quaternion q, float s)
        {
            if (s != 0.0f)
            {
                float invScalar = 1.0f / s;
                return new Quaternion(q.w * invScalar, q.x * invScalar, q.y * invScalar, q.z * invScalar);
            }
            else
            {
                return new Quaternion(float.MaxValue, float.MaxValue, float.MaxValue, float.MaxValue);
            }
        }

        public static Quaternion operator -(Quaternion q)
        {
            return new Quaternion(-q.w, -q.x, -q.y, -q.z);
        }

        public float Length
        {
            get
            {
                return Mathf.Sqrt(w * w + x * x + y * y + z * z);
            }
        }

        public float SqrLength
        {
            get
            {
                return w * w + x * x + y * y + z * z;
            }
        }

        public void Normalize()
        {
            float length = Length;
            float eps = 1e-7f;
            if (length > eps)
            {
                float invLength = 1.0f / length;
                w *= invLength;
                x *= invLength;
                y *= invLength;
                z *= invLength;
            }
            else
            {
                w = 0.0f;
                x = 0.0f;
                y = 0.0f;
                z = 0.0f;
            }
        }

        public Quaternion Inverse
        {
            get
            {
                float norm = SqrLength;
                if (norm > 0.0f)
                {
                    float invNorm = 1.0f / norm;
                    return new Quaternion(w * invNorm, -x * invNorm, -y * invNorm, -z * invNorm);
                }
                else
                {
                    return new Quaternion(0.0f, 0.0f, 0.0f, 0.0f);
                }
            }
        }

        public Quaternion Conjugate
        {
            get
            {
                return new Quaternion(w, -x, -y, -z);
            }
        }

        public float Dot(Quaternion q)
        {
            return w * q.w + x * q.x + y * q.y + z * q.z;
        }

        public static Quaternion FromAxisAngle(Vector3 axis, float angle)
        {
            float halfAngle = 0.5f * angle * Mathf.DEG2RAD;
            float sn = Mathf.Sin(halfAngle);
            float w = Mathf.Cos(halfAngle);
            float x = sn * axis[0];
            float y = sn * axis[1];
            float z = sn * axis[2];
            return new Quaternion(w, x, y, z);
        }

        public void ToAxisAngle(out Vector3 axis, out float angle)
        {
            float sqrLength = x * x + y * y + z * z;
            if (sqrLength > 0.0f)
            {
                angle = 2.0f * Mathf.Acos(w) * Mathf.RAD2DEG;
                float invLength = 1.0f / Mathf.Sqrt(sqrLength);
                axis = new Vector3(x * invLength, y * invLength, z * invLength);
            }
            else
            {
                angle = 0.0f;
                axis = new Vector3(1.0f, 0.0f, 0.0f);
            }
        }

        public static Quaternion Slerp(Quaternion from, Quaternion to, float t)
        {
            float cs = from.Dot(to);
            float angle = Mathf.Acos(cs);
            if (Mathf.Abs(angle) > 0.0f)
            {
                float sn = Mathf.Sin(angle);
                float invSn = 1.0f / sn;
                float tAngle = t * angle;
                float coeff0 = Mathf.Sin(angle - tAngle) * invSn;
                float coeff1 = Mathf.Sin(tAngle) * invSn;

                float w = coeff0 * from.w + coeff1 * to.w;
                float x = coeff0 * from.x + coeff1 * to.x;
                float y = coeff0 * from.y + coeff1 * to.y;
                float z = coeff0 * from.z + coeff1 * to.z;
                return new Quaternion(w, x, y, z);
            }
            else
            {
                return new Quaternion(from.w, from.x, from.y, from.z);
            }
        }

        public static Quaternion FromEuler(Vector3 eulerAngles)
        {
            Quaternion qx = FromAxisAngle(Vector3.UnitX, eulerAngles.X);
            Quaternion qy = FromAxisAngle(Vector3.UnitY, eulerAngles.Y);
            Quaternion qz = FromAxisAngle(Vector3.UnitZ, eulerAngles.Z);
            Quaternion result = qy * qx * qz;
            return result;
        }

        public static Quaternion Euler(Vector3 eulerAngles )
        {
            return FromEuler(eulerAngles);
        }

        public float[,] ToRotationMatrix()
        {
            float[,] r = new float[3, 3];
            r[0, 0] = 1.0f - 2.0f * y * y - 2.0f * z * z;
            r[0, 1] = 2.0f * x * y - 2.0f * w * z;
            r[0, 2] = 2.0f * x * z + 2.0f * w * y;
            r[1, 0] = 2.0f * x * y + 2.0f * w * z;
            r[1, 1] = 1.0f - 2.0f * x * x - 2.0f * z * z;
            r[1, 2] = 2.0f * y * z - 2.0f * w * x;
            r[2, 0] = 2.0f * x * z - 2.0f * w * y;
            r[2, 1] = 2.0f * y * z + 2.0f * w * x;
            r[2, 2] = 1.0f - 2.0f * x * x - 2.0f * y * y;
            return r;
        }

        public Vector3 EulerAngles
        {
            get
            {
                float[,] r = ToRotationMatrix();
                float thetaX = Mathf.Asin(-r[1, 2]);
                float thetaY = 0.0f;
                float thetaZ = 0.0f;

                if (thetaX < Math.PI * 0.5)
                {
                    if (thetaX > -Math.PI * 0.5)
                    {
                        thetaY = Mathf.Atan2(r[0, 2], r[2, 2]);
                        thetaZ = Mathf.Atan2(r[1, 0], r[1, 1]);
                    }
                    else
                    {
                        thetaY = -Mathf.Atan2(-r[0, 1], r[0, 0]);
                        thetaZ = 0.0f;
                    }
                }
                else
                {
                    thetaY = Mathf.Atan2(-r[0, 1], r[0, 0]);
                    thetaZ = 0.0f;
                }

                Vector3 v = new Vector3(thetaX, thetaY, thetaZ) * Mathf.RAD2DEG;
                //Vector3 result = new Vector3(v.X < 0 ? v.X + 360 : v.X, v.Y < 0 ? v.Y + 360 : v.Y, v.Z < 0 ? v.Z + 360 : v.Z);
                return v;
            }
        }

        public override string ToString()
        {
            return string.Format("({0:F2}, {1:F2}, {2:F2}, {3:F2})", w, x, y, z);
        }

        public static Quaternion Look(Vector3 direction)
        {
            direction.Normalize();
            Vector3 at = new Vector3(direction);
            Vector3 cp = at.Cross(-Vector3.UnitY);
            cp.Normalize();
            Vector3 right = new Vector3(cp);
            Vector3 cp2 = at.Cross(right);
            cp2.Normalize();
            Vector3 up = new Vector3(cp2);
            float[,] r = new float[3, 3];

            /*
            r[0, 0] = right.X; r[0, 1] = right.Y; r[0, 2] = right.Z;
            r[1, 0] = up.X; r[1, 1] = up.Y; r[1, 2] = up.Z;
            r[2, 0] = at.X; r[2, 1] = at.Y; r[2, 2] = at.Z;*/


            r[0, 0] = right.X; r[1, 0] = right.Y; r[2, 0] = right.Z;
            r[0, 1] = up.X; r[1, 1] = up.Y; r[2, 1] = up.Z;
            r[0, 2] = at.X; r[1, 2] = at.Y; r[2, 2] = at.Z;

            int[] next = { 1, 2, 0 };
            float trace = r[0, 0] + r[1, 1] + r[2, 2];

            if (trace > 0.0f)
            {
                float root = Mathf.Sqrt(trace + 1.0f);
                float w = 0.5f * root;
                root = 0.5f / root;
                float x = (r[2, 1] - r[1, 2]) * root;
                float y = (r[0, 2] - r[2, 0]) * root;
                float z = (r[1, 0] - r[0, 1]) * root;
                return new Quaternion(w, x, y, z);
            }
            else
            {
                int i = 0;
                if (r[1, 1] > r[0, 0])
                {
                    i = 1;
                }
                if (r[2, 2] > r[i, i])
                {
                    i = 2;
                }
                int j = next[i];
                int k = next[j];
                float root = Mathf.Sqrt(r[i, i] - r[j, j] - r[k, k] + 1.0f);
                float w = 0, x = 0, y = 0, z = 0;
                switch (i)
                {
                    case 0:
                        x = 0.5f * root;
                        break;
                    case 1:
                        y = 0.5f * root;
                        break;
                    case 2:
                        z = 0.5f * root;
                        break;
                }
                root = 0.5f / root;
                w = (r[k, j] - r[j, k]) * root;
                switch (j)
                {
                    case 0:
                        x = (r[j, i] + r[i, j]) * root;
                        break;
                    case 1:
                        y = (r[j, i] + r[i, j]) * root;
                        break;
                    case 2:
                        z = (r[j, i] + r[i, j]) * root;
                        break;
                }
                switch (k)
                {
                    case 0:
                        x = (r[k, i] + r[i, k]) * root;
                        break;
                    case 1:
                        y = (r[k, i] + r[i, k]) * root;
                        break;
                    case 2:
                        z = (r[k, i] + r[i, k]) * root;
                        break;
                }
                return new Quaternion(w, x, y, z);
            }
        }

        public static Quaternion LookRotation(Vector3 direction)
        {
            return Look(direction);
        }

        public float[] EulerArray {
            get {
                return EulerAngles.toArray();
            }
        }
    }
}
