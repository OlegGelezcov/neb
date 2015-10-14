using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using GameMath;


namespace Space.Game.Resources
{
    public class ColorInfo
    {
        /// <summary>
        /// Color of object
        /// </summary>
        public ObjectColor color;

        /// <summary>
        /// Factor of generation of charactersistics
        /// </summary>
        public float factor;

        /// <summary>
        /// probability of drop that item
        /// </summary>
        public float prob;


        public bool TryGen()
        {
            if (Rand.Float01() < this.prob)
                return true;
            return false;
        }

        public bool isBetterThanWhite {
            get {
                switch(color) {
                    case ObjectColor.blue:
                    case ObjectColor.green:
                    case ObjectColor.orange:
                    case ObjectColor.yellow:
                        return true;
                    default:
                        return false;
                }
            }
        }
    }


}
