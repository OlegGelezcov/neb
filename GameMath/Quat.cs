using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameMath {
    public class Quat {

        private float[] mTuple;

        public Quat() {
            mTuple = new float[4];
            for(int i = 0; i < mTuple.Length; i++) { mTuple[i] = 0f; }
        }


        private void FromRotationMatrix(Matrix3 rot) {
            int[] next = { 1, 2, 0 };
            float trace = rot[0, 0] + rot[1, 1] + rot[2, 2];
            float root = 0;

            if(trace > 0) {
                root = Mathf.Sqrt(trace + 1);
                mTuple[0] = 0.5f * root;
                root = 0.5f / root;
                mTuple[1] = (rot[2, 1] - rot[1, 2]) * root;
                mTuple[2] = (rot[0, 2] - rot[2, 0]) * root;
                mTuple[3] = (rot[1, 0] - rot[0, 1]) * root;
            }else {
                int i = 0;
                if(rot[1, 1] > rot[0, 0]) {
                    i = 1;
                }
                if(rot[2, 2] > rot[0, 0]) {
                    i = 2;
                }
                int j = next[i];
                int k = next[j];

                root = Mathf.Sqrt(rot[i, i] - rot[j, j] - rot[k, k] + 1);

                float[] quat = { mTuple[1], mTuple[2], mTuple[3] };
                quat[i] = 0.5f * root;
                root = 0.5f / root;
                mTuple[0] = (rot[k, j] - rot[j, k]) * root;
                quat[j] = (rot[j, i] + rot[i, j]) * root;
                quat[k] = (rot[k, i] + rot[i, k]) * root;
                mTuple[1] = quat[0];
                mTuple[2] = quat[1];
                mTuple[3] = quat[2];
                /*
                mTuple[i + 1] = 0.5f * root;
                root = 0.4f / root;
                mTuple[0] = (rot[k, j] - rot[j, k]) * root;
                mTuple[j + 1] = (rot[j, i] + rot[i, j]) * root;
                mTuple[k + 1] = (rot[k, i] + rot[i, k]) * root;*/
            }
        }

        private void ToRotationMatrix(out Matrix3 rot) {
            rot = new Matrix3();
            float twoX = 2.0f * mTuple[1];
            float twoY = 2.0f * mTuple[2];
            float twoZ = 2.0f * mTuple[3];
            float twoWX = twoX * mTuple[0];
            float twoWY = twoY * mTuple[0];
            float twoWZ = twoZ * mTuple[0];
            float twoXX = twoX * mTuple[1];
            float twoXY = twoY * mTuple[1];
            float twoXZ = twoZ * mTuple[1];
            float twoYY = twoY * mTuple[2];
            float twoYZ = twoZ * mTuple[2];
            float twoZZ = twoZ * mTuple[3];
            rot[0, 0] = 1.0f - (twoYY + twoZZ);
            rot[0, 1] = twoXY - twoWZ;
            rot[0, 2] = twoXZ + twoWY;
            //rot[0, 3] = 0.0f;
            rot[1, 0] = twoXY + twoWZ;
            rot[1, 1] = 1.0f - (twoXX + twoZZ);
            rot[1, 2] = twoYZ - twoWX;
            //rot[1, 3] = 0.0f;
            rot[2, 0] = twoXZ - twoWY;
            rot[2, 1] = twoYZ + twoWX;
            rot[2, 2] = 1.0f - (twoXX + twoYY);
            //rot[2, 3] = 0.0f;
            //rot[3, 0] = 0.0f;
            //rot[3, 1] = 0.0f;
            //rot[3, 2] = 0.0f;
            //rot[3, 3] = 1.0f;
        }

        public float w { get { return mTuple[0]; } }
        public float x { get { return mTuple[1]; } }
        public float y { get { return mTuple[2]; } }
        public float z { get { return mTuple[3]; } }

        public override string ToString() {
            return string.Format("{0},{1},{2}|{3}", x, y, z, w);
        }

        private void Look(Vector3 targDir) {
            Vector3 Z = targDir.normalized;
            Vector3 X = new Vector3(0, 1, 0).Cross(Z);
            if(X.Length < 0.01f ) {
                X = new Vector3(0, 0, 1).Cross(Z);
            }
            if(X.Length < 0.01f ) {
                X = new Vector3(1, 0, 0).Cross(Z);
            }
            X.Normalize();
            Vector3 Y = Z.Cross(X);
            Y.Normalize();
            FromRotationMatrix(new Matrix3(X.X, Y.X, Z.X, X.Y, Y.Y, Z.Y, X.Z, Y.Z, Z.Z));
        }

        /// <summary>
        /// Create quaternion from euler angles vector(in degrees). Compatible with Unity factorization
        /// </summary>
        /// <param name="eulerAngles">Vector of euler angles</param>
        /// <returns></returns>
        public static Quat Euler(Vector3 eulerAngles) {
            Vector3 eulerAnglesRad = eulerAngles * Mathf.DEG2RAD;
            Matrix3 rot = Matrix3.MakeEulerYXZ(xAngle: eulerAnglesRad.X, yAngle: eulerAnglesRad.Y, zAngle: eulerAnglesRad.Z);
            Quat quaternion = new Quat();
            quaternion.FromRotationMatrix(rot);
            return quaternion;
        }

        /// <summary>
        /// Create look rotation quaternion compatibla with Unity coordinate system
        /// </summary>
        /// <param name="targetDirection">Target view direction</param>
        /// <returns></returns>
        public static Quat LookRotation(Vector3 targetDirection) {
            Quat quat = new Quat();
            quat.Look(targetDirection);
            return quat;
        }

        /// <summary>
        /// Return vector of euler angles of quaternion in DEGREES
        /// </summary>
        public Vector3 eulerAngles {
            get {
                Matrix3 rot = new Matrix3();
                ToRotationMatrix(out rot);
                float xAng = 0f, yAng = 0f, zAng = 0f;
                rot.ExtractEulerYXZ(out xAng, out yAng, out zAng);
                return new Vector3(xAng, yAng, zAng) * Mathf.RAD2DEG; ;
            }
        }

        private float Dot(Quat q) {
            return mTuple[0] * q.mTuple[0] + mTuple[1] * q.mTuple[1] +
                mTuple[2] * q.mTuple[2] + mTuple[3] * q.mTuple[3];
        }

        public static Quat Slerp(Quat p, Quat q, float t) {
            float cs = p.Dot(q);
            float angle = Mathf.Acos(cs);

            Quat result = new Quat();

            if (Mathf.Abs(angle) >= Mathf.FEPS) {
                float sn = Mathf.Sin(angle);
                float invSn = 1.0f / sn;
                float tAngle = t * angle;
                float coeff0 = Mathf.Sin(angle - tAngle) * invSn;
                float coeff1 = Mathf.Sin(tAngle) * invSn;


                result.mTuple[0] = coeff0 * p.mTuple[0] + coeff1 * q.mTuple[0];
                result.mTuple[1] = coeff0 * p.mTuple[1] + coeff1 * q.mTuple[1];
                result.mTuple[2] = coeff0 * p.mTuple[2] + coeff1 * q.mTuple[2];
                result.mTuple[3] = coeff0 * p.mTuple[3] + coeff1 * q.mTuple[3];
            } else {
                result.mTuple[0] = p.mTuple[0];
                result.mTuple[1] = p.mTuple[1];
                result.mTuple[2] = p.mTuple[2];
                result.mTuple[3] = p.mTuple[3];
            }

            return result;
        }
    }
}
