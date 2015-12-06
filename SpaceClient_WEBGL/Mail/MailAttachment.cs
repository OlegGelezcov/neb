using Common;
using Nebula.Client.Inventory;
using ServerClientCommon;
using ExitGames.Client.Photon;
using Nebula.Client.Utils;

namespace Nebula.Client.Mail {
    public class MailAttachment : IInfoParser {
        public string id { get; private set; }
        public Hashtable objectHash { get; private set; }
        public int count { get; private set; }

        public MailAttachment(Hashtable info) { ParseInfo(info); }

        public void ParseInfo(Hashtable info) {
            id = info.GetValueString((int)SPC.Id);
            objectHash = info.GetValueHash((int)SPC.AttachedObject);
            if (info.ContainsKey((int)SPC.Count)) {
                count = info.GetValueInt((int)SPC.Count, 1);
            } else {
                count = 1;
            }
        }

        public IPlacingType ParseAttachedObject() {
            return InventoryObjectInfoFactory.GetAttachment(objectHash) as IPlacingType;
        }
    }
}
