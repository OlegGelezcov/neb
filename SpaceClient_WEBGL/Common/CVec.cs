using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common
{
    public struct CVec
    {
        public float x;
        public float y;
        public float z;

        public CVec(float iX, float iY, float iZ)
        {
            x = iX;
            y = iY;
            z = iZ;
        }

        public bool IsZero
        {
            get
            {
                return (x == 0.0f && y == 0.0f && z == 0.0f);
            }
        }

        public float Length
        {
            get
            {
                return (float)Math.Sqrt(x * x + y * y + z * z);
            }
        }

        public override string ToString()
        {
            return string.Format("({0:F1},{1:F1},{2:F1})", x, y, z);
        }
    }
}

