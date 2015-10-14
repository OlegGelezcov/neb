using Common;
using GameMath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using ServerClientCommon;

namespace Space.Game.Resources.Zones
{
    public class ZonePlanetInfo : IInfoSource
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Vector3 Position { get; set; }
        public int SlotsForStation { get; set; }
        public string PlanetWorldId { get; set; }

        public Hashtable GetInfo()
        {
            return new Hashtable
            {
                {(int)SPC.Id, this.Id},
                {(int)SPC.Name, this.Name },
                {(int)SPC.Position, this.Position.ToArray() },
                {(int)SPC.SlotCount, this.SlotsForStation },
                {(int)SPC.WorldId, this.PlanetWorldId }
            };
        }

        public bool IsNone()
        {
            return string.IsNullOrEmpty(this.Id);
        }
    }
}
