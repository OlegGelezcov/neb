// PriceFilter.cs
// Nebula
//
// Created by Oleg Zheleztsov on Monday, September 7, 2015 11:32:51 PM
// Copyright (c) 2015 KomarGames. All rights reserved.
//
using Common;

/*
Price filter for auction items in range (min price, max price)
*/
namespace SelectCharacter.Auction {
    public class PriceFilter : AuctionFilter {

        private readonly int mMinPrice;
        private readonly int mMaxPrice;

        public PriceFilter(int minPrice, int maxPrice) {
            mMinPrice = minPrice;
            mMaxPrice = maxPrice;
        }

        public override AuctionFilterType filterType {
            get {
                return AuctionFilterType.PriceFilter;
            }
        }

        public override string key {
            get {
                return filterType.ToString();
            }
        }

        public override bool Check(AuctionItem auctionItem) {
            if(auctionItem.price >= mMinPrice && auctionItem.price <= mMaxPrice) {
                return true;
            }
            return false;
        }
    }
}
