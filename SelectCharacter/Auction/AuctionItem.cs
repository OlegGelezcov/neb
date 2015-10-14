using Common;
using MongoDB.Bson;
using ServerClientCommon;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelectCharacter.Auction {
    public class AuctionItem : IInfoSource {
        public ObjectId Id { get; set; }

        public string storeItemID { get; set; }
        public string login { get; set; }
        public string gameRefID { get; set; }
        public string characterID { get; set; }
        public int count { get; set; }
        public Hashtable objectInfo { get; set; }
        public long time { get; set; }
        public int price { get; set; }

        public Hashtable GetInfo() {
            Hashtable info = new Hashtable {
                { (int)SPC.Id, storeItemID },
                { (int)SPC.Login, login },
                { (int)SPC.GameRefId, gameRefID },
                { (int)SPC.CharacterId, characterID },
                { (int)SPC.Count, count },
                { (int)SPC.Info, objectInfo },
                { (int)SPC.Time, (int)time },
                { (int)SPC.Price, price }
            };
            return info;
        }
    }
}
