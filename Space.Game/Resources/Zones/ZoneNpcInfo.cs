using Common;
using Nebula.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Space.Game.Resources.Zones
{
    /// <summary>
    /// Info about npc, which created in zone
    /// </summary>
    public class ZoneNpcInfo
    {
        /// <summary>
        /// Npc id
        /// </summary>
        public string Id { get; set; }
        public string name { get; set; }
        public int level { get; set; }
        public Race Race { get; set; }

        /// <summary>
        /// Npc spawn position
        /// </summary>
        public float[] Position { get; set; }

        /// <summary>
        /// Npc spawn rotation
        /// </summary>
        public float[] Rotation { get; set; }

        public float RespawnInterval { get; set; }

        public Workshop Workshop { get; set; }

        public Difficulty Difficulty { get; set; }

        public FractionType fraction { get; set; }

        public AIType aiType { get; set; }

    }
}
