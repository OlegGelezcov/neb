using Common;
using ExitGames.Client.Photon;
using Nebula.Client.Utils;

namespace ServerClientCommon {
    public class ConsumableItem : IInfoSource, IInfoParser {

        public ConsumableItem(string id, InventoryObjectType type, int price, int count, MoneyType moneyType) {
            this.id = id;
            this.type = type;
            this.price = price;
            this.count = count;
            this.moneyType = moneyType;
        }

        public ConsumableItem(Hashtable info) {
            ParseInfo(info);
        }

        public string id { get; private set; }
        public InventoryObjectType type { get; private set; }
        public int price { get; private set; }
        public int count { get; private set; }
        public MoneyType moneyType { get; private set; }

        //private Dictionary

        public Hashtable GetInfo() {
            return new Hashtable {
                { (int)SPC.Id, id },
                { (int)SPC.Type, (byte)type },
                { (int)SPC.Price, price },
                { (int)SPC.Count, count },
                { (int)SPC.MoneyType, (byte)moneyType }
            };
        }

        public void ParseInfo(Hashtable info) {
            id = info.GetValueString((int)SPC.Id);
            type = (InventoryObjectType)info.GetValueByte((int)SPC.Type, (byte)InventoryObjectType.repair_patch);
            price = info.GetValueInt((int)SPC.Price);
            count = info.GetValueInt((int)SPC.Count);
            moneyType = (MoneyType)info.GetValueByte((int)SPC.MoneyType, (byte)MoneyType.credits);
        }
    }
}
