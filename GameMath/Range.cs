using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameMath
{
    public class Range<T> where T : IComparable<T>
    {
        private T min;
        private T max;

        public Range(T min, T max)
        {
            this.min = min;
            this.max = max;
        }

        public T Min() { return this.min; }
        public T Max() { return this.max; }

        public bool InRange(T val)
        {
            if(val.CompareTo(this.Min()) >= 0 && val.CompareTo(this.Max()) <= 0 )
            {
                return true;
            }
            return false;
        }
    }
}
