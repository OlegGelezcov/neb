using System;

namespace Nebula.Inap{
    public class GoogleStoreItem : BaseStoreItem {

        public string priceEur { get; private set; }

        public GoogleStoreItem(string inId, string inName, string inDescription, string inPrice, int inDiscount, bool inAvailable, string inPriceEur, string inIcon)
            : base(inId, inName, inDescription, inPrice, inDiscount, inAvailable, inIcon) {
            priceEur = inPriceEur;
        }

        public override StoreType type {
            get {
                return StoreType.GoolePlay;
            }
        }
    }
}
