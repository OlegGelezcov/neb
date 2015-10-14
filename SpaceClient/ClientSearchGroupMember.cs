using Common;
using ServerClientCommon;
using System.Collections;

namespace Nebula.Client {
    public class ClientSearchGroupMember : IInfoParser{
        private Workshop workshop;
        private string displayName;
        private int level;
        private string itemId;

        public ClientSearchGroupMember(Hashtable info) {
            this.ParseInfo(info);
        }
        public void ParseInfo(Hashtable info) {
            this.workshop           = (Workshop)info.GetValue<byte>((int)SPC.Workshop, (byte)Common.Workshop.Arlen);
            this.displayName        = info.GetValue<string>((int)SPC.DisplayName, string.Empty);
            this.level              = info.GetValue<int>((int)SPC.Level, 0);
            this.itemId             = info.GetValue<string>((int)SPC.ItemId, string.Empty);
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
