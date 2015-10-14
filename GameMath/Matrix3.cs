using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameMath {
    public class Matrix3 {
        private float[] mEntry;

        public Matrix3() {
            mEntry = new float[9];
        }

        public Matrix3(float m00, float m01, float m02, float m10, float m11, float m12, float m20, float m21, float m22) {
            mEntry = new float[9];
            mEntry[0] = m00;
            mEntry[1] = m01;
            mEntry[2] = m02;
            mEntry[3] = m10;
            mEntry[4] = m11;
            mEntry[5] = m12;
            mEntry[6] = m20;
            mEntry[7] = m21;
            mEntry[8] = m22;
        }

        private Matrix3 Mult(Matrix3 mat) {
            return new Matrix3(mEntry[0] * mat.mEntry[0] +
                                mEntry[1] * mat.mEntry[3] +
                                mEntry[2] * mat.mEntry[6],

                                mEntry[0] * mat.mEntry[1] +
                                mEntry[1] * mat.mEntry[4] +
                                mEntry[2] * mat.mEntry[7],

                                mEntry[0] * mat.mEntry[2] +
                                mEntry[1] * mat.mEntry[5] +
                                mEntry[2] * mat.mEntry[8],

                                mEntry[3] * mat.mEntry[0] +
                                mEntry[4] * mat.mEntry[3] +
                                mEntry[5] * mat.mEntry[6],

                                mEntry[3] * mat.mEntry[1] +
                                mEntry[4] * mat.mEntry[4] +
                                mEntry[5] * mat.mEntry[7],

                                mEntry[3] * mat.mEntry[2] +
                                mEntry[4] * mat.mEntry[5] +
                                mEntry[5] * mat.mEntry[8],

                                mEntry[6] * mat.mEntry[0] +
                                mEntry[7] * mat.mEntry[3] +
                                mEntry[8] * mat.mEntry[6],

                                mEntry[6] * mat.mEntry[1] +
                                mEntry[7] * mat.mEntry[4] +
                                mEntry[8] * mat.mEntry[7],

                                mEntry[6] * mat.mEntry[2] +
                                mEntry[7] * mat.mEntry[5] +
                                mEntry[8] * mat.mEntry[8]
                            );
        }

        public static Matrix3 operator*(Matrix3 m1, Matrix3 m2) {
            return m1.Mult(m2);
        }

        /*
        public static Matrix3 MakeEulerXYZ(float xAngle, float yAngle, float zAngle) {
            float cs = Mathf.Cos(xAngle);
            float sn = Mathf.Sin(xAngle);
            Matrix3 xMat = new Matrix3(
                1, 0, 0,
                0, cs, -sn,
                0, sn, cs);
            cs = Mathf.Cos(yAngle);
            sn = Mathf.Sin(yAngle);
            Matrix3 yMat = new Matrix3(
                cs, 0,  sn,
                0, 1, 0,
                -sn, 0, cs);
            cs = Mathf.Cos(zAngle);
            sn = Mathf.Sin(zAngle);
            Matrix3 zMat = new Matrix3(
                cs, -sn, 0,
                sn, cs, 0,
                0, 0, 1
                );
            //return xMat * (yMat * zMat);
            //return (xMat * yMat) * zMat;
            //return (xMat * zMat) * yMat;
            return yMat * (xMat * zMat);
        }

        public EulerResult ExtractEulerXYZ(out float xAngle, out float yAngle, out float zAngle) {
            if(mEntry[2] < 1) {
                if(mEntry[2] > -1 ) {
                    yAngle = Mathf.Asin(mEntry[2]);
                    xAngle = Mathf.Atan2(-mEntry[5], mEntry[8]);
                    zAngle = Mathf.Atan2(-mEntry[1], mEntry[0]);

                    if (xAngle < 0) {
                        xAngle += Mathf.PI * 2.0f;
                    }
                    if (yAngle < 0) {
                        yAngle += Mathf.PI * 2.0f;
                    }
                    if (zAngle < 0) {
                        zAngle += Mathf.PI * 2.0f;
                    }

                    return EulerResult.Unique;
                } else {
                    yAngle = -Mathf.HALF_PI;
                    xAngle = -Mathf.Atan2(mEntry[3], mEntry[4]);
                    zAngle = 0f;
                    if (xAngle < 0) {
                        xAngle += Mathf.PI * 2.0f;
                    }
                    if (yAngle < 0) {
                        yAngle += Mathf.PI * 2.0f;
                    }
                    if (zAngle < 0) {
                        zAngle += Mathf.PI * 2.0f;
                    }

                    return EulerResult.NotUniqueDiff;
                }
            } else {
                yAngle = Mathf.HALF_PI;
                xAngle = Mathf.Atan2(mEntry[3], mEntry[4]);
                zAngle = 0;

                if (xAngle < 0) {
                    xAngle += Mathf.PI * 2.0f;
                }
                if (yAngle < 0) {
                    yAngle += Mathf.PI * 2.0f;
                }
                if (zAngle < 0) {
                    zAngle += Mathf.PI * 2.0f;
                }
                return EulerResult.NotUniqueSum;
            }


        } */

        public float this[int r, int c] {
            get {
                return mEntry[r * 3 + c];
            }
            set {
                mEntry[r * 3 + c] = value;
            }
        }

        public float[] forward {
            get {
                float len = Mathf.Sqrt(mEntry[2] * mEntry[2] + mEntry[5] * mEntry[5] + mEntry[8] * mEntry[8]);
                return new float[] { mEntry[2] / len, mEntry[5] / len , mEntry[8] / len };
                //float len = Mathf.Sqrt(mEntry[6] * mEntry[6] + mEntry[7] * mEntry[7] + mEntry[8] * mEntry[8]);
                //return new float[] { mEntry[6] / len, mEntry[7] / len, mEntry[8] / len };
            }
        }

        public override string ToString() {
            System.Text.StringBuilder builder = new StringBuilder();
            builder.AppendFormat("{0:F4}  {1:F4}  {2:F4}{3}", mEntry[0], mEntry[1], mEntry[2], Environment.NewLine);
            builder.AppendFormat("{0:F4}  {1:F4}  {2:F4}{3}", mEntry[3], mEntry[4], mEntry[5], Environment.NewLine);
            builder.AppendFormat("{0:F4}  {1:F4}  {2:F4}{3}", mEntry[6], mEntry[7], mEntry[8], Environment.NewLine);
            return builder.ToString();
        }
        /*
        public static Matrix3 MakeEulerXZY(float xAngle, float zAngle, float yAngle) {
            float cs = Mathf.Cos(xAngle);
            float sn = Mathf.Sin(xAngle);
            Matrix3 xMat = new Matrix3(
                1, 0, 0,
                0, cs, -sn,
                0, sn, cs
                );
            cs = Mathf.Cos(zAngle);
            sn = Mathf.Sin(zAngle);
            Matrix3 zMat = new Matrix3(
                cs, -sn, 0,
                sn, cs, 0,
                0, 0, 1
                );
            cs = Mathf.Cos(yAngle);
            sn = Mathf.Sin(yAngle);
            Matrix3 yMat = new Matrix3(
                cs, 0, sn,
                0, 1, 0,
                -sn, 0, cs
                );
            return xMat * (zMat * yMat);
        }*/

        public static Matrix3 MakeEulerYXZ(float yAngle, float xAngle, float zAngle) {
            float cs = Mathf.Cos(yAngle);
            float sn = Mathf.Sin(yAngle);
            Matrix3 yMat = new Matrix3(
                cs, 0, sn,
                0, 1, 0,
                -sn, 0, cs
                );
            cs = Mathf.Cos(xAngle);
            sn = Mathf.Sin(xAngle);
            Matrix3 xMat = new Matrix3(
                1, 0, 0,
                0, cs, -sn,
                0, sn, cs
                );

            cs = Mathf.Cos(zAngle);
            sn = Mathf.Sin(zAngle);
            Matrix3 zMat = new Matrix3(
                cs, -sn, 0,
                sn, cs, 0,
                0, 0, 1
                );
            return yMat * (xMat * zMat);
        }

        public EulerResult ExtractEulerYXZ(out float xAngle, out float yAngle, out float zAngle) {
            if(mEntry[5] < 1) {

                if(mEntry[5] > -1) {
                    xAngle = Mathf.Asin(-mEntry[5]);
                    yAngle = Mathf.Atan2(mEntry[2], mEntry[8]);
                    zAngle = Mathf.Atan2(mEntry[3], mEntry[4]);
                    ClampAngle(ref xAngle); ClampAngle(ref yAngle); ClampAngle(ref zAngle);

                    return EulerResult.Unique;
                } else {
                    xAngle = Mathf.HALF_PI;
                    yAngle = -Mathf.Atan2(-mEntry[1], mEntry[0]);
                    zAngle = 0;
                    ClampAngle(ref xAngle); ClampAngle(ref yAngle); ClampAngle(ref zAngle);
                    return EulerResult.NotUniqueDiff;
                }
            } else {
                xAngle = -Mathf.HALF_PI;
                yAngle = Mathf.Atan2(-mEntry[1], mEntry[0]);
                zAngle = 0;
                ClampAngle(ref xAngle); ClampAngle(ref yAngle); ClampAngle(ref zAngle);
                return EulerResult.NotUniqueSum;
            }
        }

        private void ClampAngle(ref float ang) {
            if(ang < 0f && Mathf.Abs(ang) > Mathf.FEPS) {
                ang += Mathf.PI * 2.0f;
            }
        }


        /*
        public static Matrix3 MakeEulerYZX(float yAngle, float zAngle, float xAngle) {
            float cs = Mathf.Cos(yAngle);
            float sn = Mathf.Sin(yAngle);
            Matrix3 yMat = new Matrix3(
                cs, 0, sn,
                0, 1, 0,
                -sn, 0, cs
                );
            cs = Mathf.Cos(zAngle);
            sn = Mathf.Sin(zAngle);
            Matrix3 zMat = new Matrix3(
                cs, -sn, 0,
                sn, cs, 0,
                0, 0, 1
                );

            cs = Mathf.Cos(xAngle);
            sn = Mathf.Sin(xAngle);
            Matrix3 xMat = new Matrix3(
                1, 0, 0,
                0, cs, -sn,
                0, sn, cs
                );
            return yMat * (zMat * xMat);
        }

        public static Matrix3 MakeEulerZXY(float zAngle, float xAngle, float yAngle) {
            float cs = Mathf.Cos(zAngle);
            float sn = Mathf.Sin(zAngle);
            Matrix3 zMat = new Matrix3(
                            cs, -sn, 0,
                            sn, cs, 0,
                            0, 0, 1
                            );
            cs = Mathf.Cos(xAngle);
            sn = Mathf.Sin(xAngle);
            Matrix3 xMat = new Matrix3(
                1, 0, 0,
                0, cs, -sn,
                0, sn, cs
                );
            cs = Mathf.Cos(yAngle);
            sn = Mathf.Sin(yAngle);
            Matrix3 yMat = new Matrix3(
                cs, 0, sn,
                0, 1, 0,
                -sn, 0, cs
                );

            return zMat * (xMat * yMat);
        }*/


    }

    public enum EulerResult {
        Unique,
        NotUniqueDiff,
        NotUniqueSum
    }
}
