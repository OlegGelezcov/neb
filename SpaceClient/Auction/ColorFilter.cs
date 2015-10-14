// ColorFilter.cs
// Nebula
//
// Created by Oleg Zheleztsov on Monday, September 7, 2015 11:43:29 PM
// Copyright (c) 2015 KomarGames. All rights reserved.
//
using Common;
using ServerClientCommon;
using System.Collections;

namespace Nebula.Client.Auction {
    public class ColorFilter : AuctionFilter {

        public ObjectColor color { get; set; }

        public override AuctionFilterType filterType {
            get {
                return AuctionFilterType.ColorFilter;
            }
        }
        public override Hashtable GetInfo() {
            return new Hashtable {
                {(int)SPC.Type, (byte)filterType },
                {(int)SPC.Color, (byte)color }
            };
        }

        public override string key {
            get {
                return Key(color);
            }
        }

        public static string Key(ObjectColor color) {
            return AuctionFilterType.ColorFilter.ToString() + color.ToString();
        }

        public override string ToString() {
            return string.Format("[{0}:{1}]", filterType, color);
        }
    }
}
