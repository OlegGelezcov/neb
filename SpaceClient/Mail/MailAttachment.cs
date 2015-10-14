using Common;
using Nebula.Client.Inventory;
using ServerClientCommon;
using System.Collections;

namespace Nebula.Client.Mail {
    public class MailAttachment : IInfoParser {
        public string id { get; private set; }
        public Hashtable objectHash { get; private set; }
        public int count { get; private set; }

        public MailAttachment(Hashtable info) { ParseInfo(info); }

        public void ParseInfo(Hashtable info) {
            id = info.Value<string>((int)SPC.Id);
            objectHash = info.Value<Hashtable>((int)SPC.AttachedObject);
            if(info.ContainsKey((int)SPC.Count)) {
                count = info.GetValue<int>((int)SPC.Count, 1);
            } else {
                count = 1;
            }
        }

        public IPlacingType ParseAttachedObject() {
            return InventoryObjectInfoFactory.GetAttachment(objectHash) as IPlacingType;
        }
    }
}
