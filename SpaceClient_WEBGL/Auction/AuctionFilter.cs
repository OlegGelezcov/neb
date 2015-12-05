// AuctionFilter.cs
// Nebula
//
// Created by Oleg Zheleztsov on Monday, September 7, 2015 11:45:14 PM
// Copyright (c) 2015 KomarGames. All rights reserved.
//
using Common;
using ExitGames.Client.Photon;

namespace Nebula.Client.Auction {
    public abstract class AuctionFilter : IInfoSource {
        public abstract AuctionFilterType filterType { get; }

        public abstract Hashtable GetInfo();

        public abstract string key { get; }
    }
}
