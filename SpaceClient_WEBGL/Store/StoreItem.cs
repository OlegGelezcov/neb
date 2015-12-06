using Common;
using ServerClientCommon;
using ExitGames.Client.Photon;
using Nebula.Client.Utils;

namespace Nebula.Client.Store {
    public class StoreItem : IInfoParser {
        public string storeItemID { get; private set; } = string.Empty;
        public int count { get; private set; } = 0;
        public Hashtable objectInfo { get; private set; } = new Hashtable();
        public long time { get; private set; } = 0;
        public int price { get; private set; } = 0;

        public void ParseInfo(Hashtable info) {
            storeItemID = info.GetValueString((int)SPC.Id);
            count = info.GetValueInt((int)SPC.Count);
            objectInfo = info.GetValueHash((int)SPC.AttachedObject);
            time = info.GetValueInt((int)SPC.Time);
            price = info.GetValueInt((int)SPC.Price);
        }

        public StoreItem() {

        }

        public StoreItem(Hashtable info) {
            ParseInfo(info);
        }
    }
}
