// AuctionItem.cs
// Nebula
//
// Created by Oleg Zheleztsov on Monday, September 7, 2015 11:44:41 PM
// Copyright (c) 2015 KomarGames. All rights reserved.
//
using Common;
using ServerClientCommon;
using ExitGames.Client.Photon;
using Nebula.Client.Utils;

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
            storeItemId = info.GetValueString((int)SPC.Id);
            login = info.GetValueString((int)SPC.Login);
            gameRefID = info.GetValueString((int)SPC.GameRefId);
            characterID = info.GetValueString((int)SPC.CharacterId);
            count = info.GetValueInt((int)SPC.Count);
            objectInfo = info.GetValueHash((int)SPC.Info);
            time = info.GetValueInt((int)SPC.Time);
            price = info.GetValueInt((int)SPC.Price);
        }

        public AuctionItem() { }
        public AuctionItem(Hashtable info) {
            ParseInfo(info);
        }


    }


}
