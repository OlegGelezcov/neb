// AuctionFilter.cs
// Nebula
//
// Created by Oleg Zheleztsov on Monday, September 7, 2015 11:18:13 PM
// Copyright (c) 2015 KomarGames. All rights reserved.
//
using Common;
using ExitGames.Logging;
using ServerClientCommon;
using System.Collections;
using System.Collections.Generic;

namespace SelectCharacter.Auction {
    public abstract class AuctionFilter {
        private static ILogger log = LogManager.GetCurrentClassLogger();

        public abstract bool Check(AuctionItem objectInfo);
        public abstract AuctionFilterType filterType { get;  }

        /// <summary>
        /// Unique key for differrentiate auctions
        /// </summary>
        public abstract string key { get; }

        //Filters factory method
        public static AuctionFilter Get(Hashtable filterHash ) {
            AuctionFilterType filterType = (AuctionFilterType)(byte)filterHash.GetValue<byte>((int)SPC.Type, (byte)AuctionFilterType.ColorFilter);
            switch(filterType) {
                case AuctionFilterType.ObjectTypeFilter:
                    {
                        AuctionObjectType objectType = (AuctionObjectType)(byte)filterHash.GetValue<byte>((int)SPC.ObjectType, (byte)AuctionObjectType.Module);
                        return new AuctionObjectTypeFilter(objectType);
                    }
                case AuctionFilterType.ColorFilter:
                    {
                        ObjectColor objectColor = (ObjectColor)(byte)filterHash.GetValue<byte>((int)SPC.Color, (byte)ObjectColor.blue);
                        return new ColorFilter(objectColor);
                    }
                case AuctionFilterType.WorkshopFilter:
                    {
                        Workshop workshop = (Workshop)(byte)filterHash.Value<byte>((int)SPC.Workshop, (byte)Workshop.Arlen);
                        return new WorkshopFilter(workshop);
                    }
                case AuctionFilterType.PriceFilter:
                    {
                        int minPrice = filterHash.GetValue<int>((int)SPC.MinPrice, 0);
                        int maxPrice = filterHash.GetValue<int>((int)SPC.MaxPrice, int.MaxValue);
                        return new PriceFilter(minPrice, maxPrice);
                    }
                case AuctionFilterType.LevelFilter:
                    {
                        int minLevel = filterHash.GetValue<int>((int)SPC.MinLevel, 0);
                        int maxLevel = filterHash.GetValue<int>((int)SPC.MaxLevel, int.MaxValue);
                        return new LevelFilter(minLevel, maxLevel);
                    }
                default:
                    {
                        log.ErrorFormat("unsupported filter = {0}", filterType);
                        return null;
                    }
            }
        }

        public static List<AuctionFilter> GetList(Hashtable filterHash) {
            List<AuctionFilter> newFilters = new List<AuctionFilter>();
            foreach(DictionaryEntry entry in filterHash ) {
                newFilters.Add(Get(entry.Value as Hashtable));
            }
            return newFilters;
        }
    }
}
