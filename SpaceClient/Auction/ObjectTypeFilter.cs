// ObjectTypeFilter.cs
// Nebula
//
// Created by Oleg Zheleztsov on Monday, September 7, 2015 11:42:43 PM
// Copyright (c) 2015 KomarGames. All rights reserved.
//
using Common;
using ServerClientCommon;
using System.Collections;

namespace Nebula.Client.Auction {
    public class ObjectTypeFilter : AuctionFilter {

        public AuctionObjectType objectType { get; set; }

        public override AuctionFilterType filterType {
            get {
                return AuctionFilterType.ObjectTypeFilter;
            }
        }

        public override Hashtable GetInfo() {
            return new Hashtable {
                {(int)SPC.Type, (byte)filterType },
                {(int)SPC.ObjectType, (byte)objectType }
            };
        }

        public override string key {
            get {
                return Key(objectType);
            }
        }

        public static string Key(AuctionObjectType objType) {
            return AuctionFilterType.ObjectTypeFilter.ToString() + objType.ToString();
        }

        public override string ToString() {
            return string.Format("[{0}:{1}]", filterType, objectType);
        }
    }
}
