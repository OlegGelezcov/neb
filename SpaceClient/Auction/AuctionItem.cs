// AuctionItem.cs
// Nebula
//
// Created by Oleg Zheleztsov on Monday, September 7, 2015 11:44:41 PM
// Copyright (c) 2015 KomarGames. All rights reserved.
//
using Common;
using ServerClientCommon;
using System.Collections;

namespace Nebula.Client.Auction {
    public class AuctionItem : IInfoParser {
        public string storeItemId { get; private set; }
        public string login { get; private set; }
        public string gameRefID { get; private set; }
        public string characterID { get; private set; }
        public int count { get; private set; }
        public Hashtable objectInfo { get; private set; }
        public long time { get; private set; }
        public int price { get; private set; }

        public void ParseInfo(Hashtable info) {
            storeItemId =   info.Value<string>((int)SPC.Id, string.Empty);
            login =         info.Value<string>((int)SPC.Login, string.Empty);
            gameRefID =     info.Value<string>((int)SPC.GameRefId, string.Empty);
            characterID =   info.Value<string>((int)SPC.CharacterId, string.Empty);
            count =         info.Value<int>((int)SPC.Count, 0);
            objectInfo =    info.Value<Hashtable>((int)SPC.Info, new Hashtable());
            time =          info.Value<int>((int)SPC.Time, 0);
            price =         info.Value<int>((int)SPC.Price, 0);
        }

        public AuctionItem() { }
        public AuctionItem(Hashtable info) {
            ParseInfo(info);
        }

        
    }


}
