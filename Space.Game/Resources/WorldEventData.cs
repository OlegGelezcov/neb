using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using GameMath;

namespace Space.Game.Resources
{
    public class WorldEventData
    {
        public string Id { get; set; }
        public float Cooldown { get; set; }
        public float Radius { get; set; }
        public Vector3 Position { get; set; }
        public Hashtable Inputs { get; set; }
    }
}
