using Common;
using ServerClientCommon;
using ExitGames.Client.Photon;
using Nebula.Client.Utils;

namespace Nebula.Client {
    public class ClientSearchGroupMember : IInfoParser {
        private Workshop workshop;
        private string displayName;
        private int level;
        private string itemId;

        public ClientSearchGroupMember(Hashtable info) {
            this.ParseInfo(info);
        }
        public void ParseInfo(Hashtable info) {
            this.workshop = (Workshop)info.GetValueByte((int)SPC.Workshop, (byte)Common.Workshop.Arlen);
            this.displayName = info.GetValueString((int)SPC.DisplayName);
            this.level = info.GetValueInt((int)SPC.Level);
            this.itemId = info.GetValueString((int)SPC.ItemId);
        }
        public Workshop Workshop() {
            return this.workshop;
        }
        public string DisplayName() {
            return this.displayName;
        }
        public int Level() {
            return this.level;
        }
        public string ItemId() {
            return this.itemId;
        }
    }
}
