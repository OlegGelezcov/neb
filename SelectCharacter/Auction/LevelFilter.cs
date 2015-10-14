// LevelFilter.cs
// Nebula
//
// Created by Oleg Zheleztsov on Monday, September 7, 2015 11:38:36 PM
// Copyright (c) 2015 KomarGames. All rights reserved.
//
using Common;
using ServerClientCommon;
using System.Collections;

/*Level filter for auction item requests*/
namespace SelectCharacter.Auction {
    public class LevelFilter : AuctionFilter {
        private readonly int mMinLevel;
        private readonly int mMaxLevel;

        public LevelFilter(int minLevel, int maxLevel) {
            mMinLevel = minLevel;
            mMaxLevel = maxLevel;
        }

        public override AuctionFilterType filterType {
            get {
                return AuctionFilterType.LevelFilter;
            }
        }

        public override string key {
            get {
                return filterType.ToString();
            }
        }

        public override bool Check(AuctionItem auctionItem) {
            Hashtable objectInfo = auctionItem.objectInfo;
            if(objectInfo != null ) {
                if(objectInfo.ContainsKey((int)SPC.Level)) {
                    int itemLevel = objectInfo.GetValue<int>((int)SPC.Level, 0);
                    if(itemLevel >= mMinLevel && itemLevel <= mMaxLevel) {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
