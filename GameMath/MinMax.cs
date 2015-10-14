using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameMath
{
    [System.Serializable]
    public struct MinMax
    {
        public Vector3 min;
        public Vector3 max;

        public MinMax(Vector3 min, Vector3 max)
        {
            this.min = min;
            this.max = max;
        }

        public Vector3 Min
        {
            get
            {
                return this.min;
            }
        }

        public Vector3 Max
        {
            get
            {
                return this.max;
            }
        }

        public override string ToString()
        {
            return string.Format("[{0:F1},{1:F1},{2:F1};{3:F1},{4:F1},{5:F1}]", min.X, min.Y, min.Z, max.X, max.Y, max.Z);
        }
    }
}
