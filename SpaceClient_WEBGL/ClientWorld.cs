
using Common;
using ServerClientCommon;
using ExitGames.Client.Photon;
using Nebula.Client.Utils;

namespace Nebula.Client {
    public class ClientWorld : IInfo {
        public string Id { get; private set; }
        public Race OwnedRace { get; private set; }
        public ClientWorldZone Zone { get; private set; }

        public ClientWorld() {

        }

        public ClientWorld(Hashtable info) {
            this.ParseInfo(info);
        }

        public Hashtable GetInfo() {
            Hashtable info = new Hashtable();
            info.Add((int)SPC.Id, this.Id);
            info.Add((int)SPC.OwnedRace, this.OwnedRace.toByte());
            info.Add((int)SPC.ZoneInfo, (this.Zone != null) ? this.Zone.GetInfo() : new Hashtable());
            return info;
        }

        public void ParseInfo(Hashtable info) {
            this.Id = info.GetValueString((int)SPC.Id);
            this.OwnedRace = (Race)info.GetValueByte((int)SPC.OwnedRace);
            this.Zone = new ClientWorldZone(info.GetValueHash((int)SPC.ZoneInfo));
        }
    }
}
