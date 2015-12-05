
using Common;
using ServerClientCommon;
using ExitGames.Client.Photon;
using Nebula.Client.Utils;

namespace Nebula.Client {
    public class ClientWorldZone : IInfo {
        public string Id { get; private set; }
        public string Name { get; private set; }
        public int Level { get; private set; }
        public Race InitiallyOwnedRace { get; private set; }

        public ClientWorldZone() {

        }
        public ClientWorldZone(Hashtable info) {
            this.ParseInfo(info);
        }

        public Hashtable GetInfo() {
            Hashtable info = new Hashtable();
            info.Add((int)SPC.Id, this.Id);
            info.Add((int)SPC.DisplayName, this.Name);
            info.Add((int)SPC.Level, this.Level);
            info.Add((int)SPC.InitialOwnedRace, this.InitiallyOwnedRace.toByte());
            return info;
        }

        public void ParseInfo(Hashtable info) {
            this.Id = info.GetValueString((int)SPC.Id);
            this.Name = info.GetValueString((int)SPC.DisplayName);
            this.Level = info.GetValueInt((int)SPC.Level);
            this.InitiallyOwnedRace = (Race)info.GetValueByte((int)SPC.InitialOwnedRace);
        }
    }
}
