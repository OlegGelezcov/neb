// WorkshopFilter.cs
// Nebula
//
// Created by Oleg Zheleztsov on Monday, September 7, 2015 11:42:03 PM
// Copyright (c) 2015 KomarGames. All rights reserved.
//
using Common;
using ServerClientCommon;
using System.Collections;

namespace Nebula.Client.Auction {
    public class WorkshopFilter : AuctionFilter {

        public Workshop workshop { get; set; }

        public override AuctionFilterType filterType {
            get {
                return AuctionFilterType.WorkshopFilter;
            }
        }

        public override Hashtable GetInfo() {
            return new Hashtable {
                {(int)SPC.Type, (byte)filterType },
                {(int)SPC.Workshop, (byte)workshop }
            };
        }

        public override string key {
            get {
                return Key(workshop);
            }
        }

        public static string Key(Workshop workshop) {
            return AuctionFilterType.WorkshopFilter.ToString() + workshop.ToString();
        }

        public override string ToString() {
            return string.Format("[{0}:{1}]", filterType, workshop);
        }
    }
}
