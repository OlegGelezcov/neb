using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Common
{
    public struct SphericalCoord
    {
        private float radius;
        private float theta;
        private float phi;

        public float R
        {
            get
            {
                return this.radius;
            }
            set
            {
                this.radius = value;
            }
        }

        public float Thteta
        {
            get
            {
                return this.theta;
            }
            set
            {
                this.theta = value;
            }
        }

        public float Phi
        {
            get
            {
                return this.phi;
            }
            set
            {
                this.phi = value;
            }
        }

        public SphericalCoord(float radius, float theta, float phi)
        {
            this.radius = radius;
            this.theta = theta;
            this.phi = phi;
        }

        public override string ToString()
        {
            return string.Format("({0:F1},{1:F1},{2:F1})", radius, theta, phi);
        }
    }
}
