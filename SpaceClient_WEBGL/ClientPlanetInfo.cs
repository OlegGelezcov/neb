using Common;
using ExitGames.Client.Photon;
using Nebula.Client.Utils;
using ServerClientCommon;

namespace Nebula.Client {
    public class ClientPlanetInfo : IInfoParser {
        public string Id { get; private set; }
        public string Name { get; private set; }
        public float[] Position { get; private set; }
        public int SlotsForStation { get; private set; }
        public string PlanetWorldId { get; private set; }

        public void ParseInfo(Hashtable info) {
            this.Id = info.GetValueString((int)SPC.Id);
            this.Name = info.GetValueString((int)SPC.Name);
            this.Position = info.GetValueFloatArray((int)SPC.Position);
            this.SlotsForStation = info.GetValueInt((int)SPC.SlotCount);
            this.PlanetWorldId = info.GetValueString((int)SPC.WorldId);
        }

        public ClientPlanetInfo(Hashtable info) {
            this.ParseInfo(info);
        }

        public ClientPlanetInfo() { }
    }
}
