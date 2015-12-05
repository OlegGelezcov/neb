// LevelFilter.cs
// Nebula
//
// Created by Oleg Zheleztsov on Tuesday, September 8, 2015 12:07:30 AM
// Copyright (c) 2015 KomarGames. All rights reserved.
//
using Common;
using ServerClientCommon;
using ExitGames.Client.Photon;

namespace Nebula.Client.Auction {
    public class LevelFilter : AuctionFilter {

        public int minLevel { get; set; }
        public int maxLevel { get; set; }

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

        public override Hashtable GetInfo() {
            return new Hashtable {
                { (int)SPC.Type, (byte)filterType },
                { (int)SPC.MinLevel, minLevel },
                { (int)SPC.MaxLevel, maxLevel }
            };
        }
    }
}
