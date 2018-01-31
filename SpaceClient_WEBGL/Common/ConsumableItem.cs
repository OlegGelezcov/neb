using Common;
using ExitGames.Client.Photon;
using Nebula.Client.Inaps;

namespace ServerClientCommon {
    public class ConsumableItem : IInfoSource, IInfoParser, IInapObject {

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

        #region IInapObject
        public string Icon {
            get {
                return string.Empty;
            }
        }

        public string Name {
            get {
                return string.Empty;
            }
        }

        public string Description {
            get {
                return string.Empty;
            }
        }

        public int Price {
            get {
                return price;
            }
        }

        public CoinType CoinType {
            get {
                if (moneyType == MoneyType.credits) {
                    return CoinType.credits;
                } else {
                    return CoinType.pvp_points;
                }
            }
        } 
        #endregion


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
            id = info.GetValue<string>((int)SPC.Id, string.Empty);
            type = (InventoryObjectType)info.GetValue<byte>((int)SPC.Type, (byte)InventoryObjectType.repair_patch);
            price = info.GetValue<int>((int)SPC.Price, 0);
            count = info.GetValue<int>((int)SPC.Count, 0);
            moneyType = (MoneyType)info.GetValue<byte>((int)SPC.MoneyType, (byte)MoneyType.credits);
        }
    }
}
