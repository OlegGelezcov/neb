using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Space.Game
{
    public class ExitWorkshopSavedInfo
    {
        public float[] ViewDistanceEnter { get; set; }
        public float[] ViewDistanceExit { get; set; }
        public float[] Position { get; set; }
        public float[] Rotation { get; set; }
        public Hashtable Properties { get; set; }

        public string ItemId { get; set; }

        public bool NowInWorkshop { get; set; }
    }
}
