using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using ServerClientCommon;

namespace SelectCharacter.Auction {
    public class ColorFilter : AuctionFilter {

        private readonly ObjectColor mColor;

        public ColorFilter(ObjectColor color) {
            mColor = color;
        }

        public override bool Check(AuctionItem auctionItem) {
            if (auctionItem.objectInfo.ContainsKey((int)SPC.Color)) {
                ObjectColor color = (ObjectColor)(byte)(int)auctionItem.objectInfo[(int)SPC.Color];
                if(color == mColor) {
                    return true;
                }
            }
            return false;
        }

        public override AuctionFilterType filterType {
            get {
                return AuctionFilterType.ColorFilter;
            }
        }
        public override string key {
            get {
                return filterType.ToString() + mColor.ToString();
            }
        }

        public override string ToString() {
            return string.Format("[{0}:{1}]", filterType, mColor);
        }
    }
}
