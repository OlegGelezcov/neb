// WorkshopFilter.cs
// Nebula
//
// Created by Oleg Zheleztsov on Monday, September 7, 2015 11:18:40 PM
// Copyright (c) 2015 KomarGames. All rights reserved.
//
using Common;
using ServerClientCommon;
using System.Collections;

namespace SelectCharacter.Auction {
    public class WorkshopFilter : AuctionFilter {
        private readonly Workshop mWorkshop;

        public WorkshopFilter(Workshop workshop) {
            mWorkshop = workshop;
        }

        public override bool Check(AuctionItem auctionItem) {
            if(auctionItem.objectInfo.ContainsKey((int)SPC.Workshop)) {
                Workshop workshop = (Workshop)(byte)(int)auctionItem.objectInfo[(int)SPC.Workshop];
                if(mWorkshop == workshop) {
                    return true;
                }
            }
            return false;
        }

        public override AuctionFilterType filterType {
            get {
                return AuctionFilterType.WorkshopFilter;
            }
        }

        public override string key {
            get {
                return filterType.ToString() + mWorkshop.ToString();
            }
        }

        public override string ToString() {
            return string.Format("[{0}:{1}]", filterType, mWorkshop);
        }
    }
}
