using Common;
using ServerClientCommon;
using System.Collections;

namespace SelectCharacter.Auction {
    class AuctionObjectTypeFilter : AuctionFilter {

        private readonly AuctionObjectType mAuctionObjectType;

        public AuctionObjectTypeFilter(AuctionObjectType type) {
            mAuctionObjectType = type;
        }


        public override bool Check(AuctionItem auctionItem) {
            if (auctionItem.objectInfo.ContainsKey((int)SPC.PlacingType)) {
                PlacingType placingType = (PlacingType)(int)auctionItem.objectInfo[(int)SPC.PlacingType];

                if (auctionItem.objectInfo.ContainsKey((int)SPC.ItemType)) {
                    InventoryObjectType itemType = (InventoryObjectType)(byte)(int)auctionItem.objectInfo[(int)SPC.ItemType];
                    if (itemType == InventoryObjectType.Weapon && mAuctionObjectType == AuctionObjectType.Weapon) {
                        return true;
                    } else if (itemType == InventoryObjectType.Scheme && mAuctionObjectType == AuctionObjectType.Scheme) {
                        return true;
                    } else if (itemType == InventoryObjectType.Material && mAuctionObjectType == AuctionObjectType.Ore) {
                        return true;
                    } else if(itemType == InventoryObjectType.Module && mAuctionObjectType == AuctionObjectType.Module) {
                        return true;
                    } else if(itemType == InventoryObjectType.nebula_element && mAuctionObjectType == AuctionObjectType.nebula_element ) {
                        return true;
                    } else if(itemType == InventoryObjectType.craft_resource && mAuctionObjectType == AuctionObjectType.craft_resource ) {
                        return true;
                    } else if(itemType == InventoryObjectType.pet_scheme && mAuctionObjectType == AuctionObjectType.pet_scheme) {
                        return true;
                    }
                }
            }
            return false;
        }

        public override AuctionFilterType filterType {
            get {
                return AuctionFilterType.ObjectTypeFilter;
            }
        }
        public override string key {
            get {
                return filterType.ToString() + mAuctionObjectType.ToString();
            }
        }

        public override string ToString() {
            return string.Format("[{0}:{1}]", filterType, mAuctionObjectType);
        }
    }
}
