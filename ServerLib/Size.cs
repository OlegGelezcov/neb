using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Common
{
    public struct Size
    {
        private float width;
        private float height;

        public Size(float width, float height)
        {
            this.width = width;
            this.height = height;
        }

        public float Width
        {
            get
            {
                return this.width;
            }
        }

        public float Height
        {
            get
            {
                return this.height;
            }
        }

        public void SetWidth(float width)
        {
            this.width = width;
        }

        public void SetHeight(float height)
        {
            this.height = height;
        }

        public void ChangeSize(float width, float height)
        {
            this.width = width;
            this.height = height;
        }
    }
}
