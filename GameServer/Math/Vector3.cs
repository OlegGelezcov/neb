namespace Space
{
    using Space.Server;
    using System;
    public struct Vector3
    {

        private float mx;
        private float my;
        private float mz;

        public Vector3(Vector3 v)
        {
            mx = v.mx;
            my = v.my;
            mz = v.mz;
        }

        public Vector3(float[] tuple)
        {
            mx = tuple[0];
            my = tuple[1];
            mz = tuple[2];
        }

        public Vector3(float x, float y, float z)
        {
            mx = x;
            my = y;
            mz = z;
        }

        public float X
        {
            get
            {
                return mx;
            }
            set
            {
                mx = value;
            }
        }

        public float Y
        {
            get
            {
                return my;
            }
            set
            {
                my = value;
            }
        }

        public float Z
        {
            get
            {
                return mz;
            }
            set
            {
                mz = value;
            }
        }

        public static Vector3 operator+(Vector3 left, Vector3 right)
        {
            return new Vector3(left.mx + right.mx,
                left.my + right.my,
                left.mz + right.mz);
        }

        public static Vector3 operator-(Vector3 left, Vector3 right )
        {
            return new Vector3(left.mx - right.mx,
                                left.my - right.my,
                                left.mz - right.mz);
        }

        public static Vector3 operator*(Vector3 v, float s)
        {
            return new Vector3(v.mx * s, v.my * s, v.mz * s);
        }
        public static Vector3 operator*(float s, Vector3 v)
        {
            return new Vector3(v.mx * s, v.my * s, v.mz * s);
        }

        public static Vector3 operator/(Vector3 v, float s )
        {
            if(s != 0.0f )
            {
                float invs = 1.0f / s;
                return new Vector3(v.mx * invs, v.my * invs, v.mz * invs);
            }
            else
            {
                return new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            }
        }

        public static Vector3 operator-(Vector3 v)
        {
            return new Vector3(-v.mx, -v.my, -v.mz);
        }

        public float Length
        {
            get
            {
                return (float)Math.Sqrt(mx * mx + my * my + mz * mz);
            }
        }

        public float SqrLength
        {
            get
            {
                return mx * mx + my * my + mz * mz;
            }
        }

        public float Dot(Vector3 v)
        {
            return mx * v.mx + my * v.my + mz * v.mz;
        }

        public void Normalize()
        {
            float length = Length;
            float eps = 1e-6f;
            if(length > eps )
            {
                float invLength = 1.0f / length;
                mx *= invLength;
                my *= invLength;
                mz *= invLength;
            }
            else
            {
                mx = 0.0f;
                my = 0.0f;
                mz = 0.0f;
            }
        }

        public float this[int i]
        {
            get
            {
                if (i == 0)
                    return mx;
                else if (i == 1)
                    return my;
                else if (i == 2)
                    return mz;
                else
                    throw new IndexOutOfRangeException();
            }
            set
            {
                if (i == 0)
                    mx = value;
                else if (i == 1)
                    my = value;
                else if (i == 2)
                    mz = value;
                else
                    throw new IndexOutOfRangeException();
            }
        }

        public Vector3 Cross(Vector3 v)
        {
            return new Vector3( my * v.mz - mz * v.my,
                mz * v.mx - mx * v.mz,
                mx * v.my - my * v.mx);
        }

        public Vector3 UnitCross(Vector3 v )
        {
            Vector3 cross = new Vector3(my * v.mz - mz * v.my,
                mz * v.mx - mx * v.mz,
                mx * v.my - my * v.mx);
            cross.Normalize();
            return cross;
        }

        public static void ComputeExtremes(Vector3[] vectors, out Vector3 vmin, out Vector3 vmax )
        {
            vmin = vectors[0];
            vmax = vmin;

            for(int j = 1; j < vectors.Length; j++ )
            {
                Vector3 vec = vectors[j];
                for(int i = 0; i < 3; i++ )
                {
                    if (vec[i] < vmin[i])
                        vmin[i] = vec[i];
                    else if (vec[i] > vmax[i])
                        vmax[i] = vec[i];
                }
            }
        }

        public static void Orthonormalize(ref Vector3 u, ref Vector3 v, ref Vector3 w )
        {
            u.Normalize();
            float dot0 = u.Dot(v);
            v -= dot0 * u;
            v.Normalize();

            float dot1 = v.Dot(w);
            dot0 = u.Dot(w);
            w -= dot0 * u + dot1 * v;
            w.Normalize();
        }

        public override string ToString()
        {
            return string.Format("({0:F2}, {1:F2}, {2:F2})", mx, my, mz);
        }

        public static Vector3 Zero
        {
            get
            {
                return new Vector3(0.0f, 0.0f, 0.0f);
            }
        }

        public static Vector3 UnitX
        {
            get
            {
                return new Vector3(1.0f, 0.0f, 0.0f);
            }
        }

        public static Vector3 UnitY
        {
            get
            {
                return new Vector3(0.0f, 1.0f, 0.0f);
            }
        }

        public static Vector3 UnitZ
        {
            get
            {
                return new Vector3(0.0f, 0.0f, 1.0f);
            }
        }

        public static Vector3 One
        {
            get
            {
                return new Vector3(1.0f, 1.0f, 1.0f);
            }
        }

        public Vector3 normalized
        {
            get
            {
                float length = Length;
                if (length > 0.0f)
                {
                    return new Vector3(mx / length, my / length, mz / length);
                }
                else
                {
                    return Vector3.Zero;
                }
            }
        }

        public static bool operator ==(Vector3 left, Vector3 right)
        {
            return left.mx == right.mx && left.my == right.my && left.mz == right.mz;
        }

        public static bool operator !=(Vector3 left, Vector3 right)
        {
            return !(left == right);
        }

        public static float Distance(Vector3 a, Vector3 b)
        {
            Vector3 v = new Vector3(b.mx - a.mx, b.my - a.my, b.mz - a.mz);
            return v.Length;
        }

        public float magnitude
        {
            get
            {
                return Length;
            }
        }

        public float[] toArray() {
            return new float[] { X, Y, Z };
        }

        public Vector toVec()
        {
            return new Vector { X = this.X, Y = this.Y, Z = this.Z };
        }
        
    }
}
