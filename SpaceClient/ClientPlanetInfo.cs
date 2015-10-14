using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using System.Collections;
using ServerClientCommon;

namespace Nebula.Client
{
    public class ClientPlanetInfo : IInfoParser
    {
        public string Id { get; private set; }
        public string Name { get; private set; }
        public float[] Position { get; private set; }
        public int SlotsForStation { get; private set; }
        public string PlanetWorldId { get; private set; }

        public void ParseInfo(Hashtable info)
        {
            this.Id = info.GetValue<string>((int)SPC.Id, string.Empty);
            this.Name = info.GetValue<string>((int)SPC.Name, string.Empty);
            this.Position = info.GetValue<float[]>((int)SPC.Position, new float[] { 0f, 0f, 0f });
            this.SlotsForStation = info.GetValue<int>((int)SPC.SlotCount, 0);
            this.PlanetWorldId = info.GetValue<string>((int)SPC.WorldId, string.Empty);
        }

        public ClientPlanetInfo(Hashtable info)
        {
            this.ParseInfo(info);
        }

        public ClientPlanetInfo()
        { }
    }
}
