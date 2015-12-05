// PriceFilter.cs
// Nebula
//
// Created by Oleg Zheleztsov on Monday, September 7, 2015 11:49:13 PM
// Copyright (c) 2015 KomarGames. All rights reserved.
//
using Common;
using ServerClientCommon;
using ExitGames.Client.Photon;

namespace Nebula.Client.Auction {
    public class PriceFilter : AuctionFilter {

        public int minPrice { get; set; }
        public int maxPrice { get; set; }

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

        public override Hashtable GetInfo() {
            return new Hashtable {
                { (int)SPC.Type, (byte)filterType },
                { (int)SPC.MinPrice, minPrice },
                { (int)SPC.MaxPrice, maxPrice }
            };
        }
    }
}
