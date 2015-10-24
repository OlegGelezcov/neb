using ExitGames.Logging;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using ServerClientCommon;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelectCharacter.Auction {
    public class AuctionService {

        private SelectCharacterApplication mApplication;
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private ConcurrentDictionary<string, PlayerAuctionRequest> mCursors = new ConcurrentDictionary<string, PlayerAuctionRequest>();

        public AuctionService(SelectCharacterApplication application) {
            mApplication = application;
        }

        public void Put(AuctionItem item) {
            mApplication.DB.Auction.Save(item);
        }

        public AuctionItem Remove(string storeItemID ) {
            var item = mApplication.DB.Auction.FindOne(Query<AuctionItem>.EQ(i => i.storeItemID, storeItemID));
            if(item != null ) {
                mApplication.DB.Auction.Remove(Query<AuctionItem>.EQ(i => i.storeItemID, storeItemID));
            }
            return item;
        }

        /// <summary>
        /// Find and return one item from auction with store ID
        /// </summary>
        public AuctionItem GetItem(string storeItemID ) {
            var item = mApplication.DB.Auction.FindOne(Query<AuctionItem>.EQ(i => i.storeItemID, storeItemID));
            return item;
        }

        public void WriteItem(AuctionItem item) {
            mApplication.DB.Auction.Save(item);
        }


        public PlayerAuctionRequest GetOrCreatePlayerAuctionRequest(string characterID ) {
            PlayerAuctionRequest request = null;
            if(mCursors.TryGetValue(characterID, out request)) {
                return request;
            } else {
                request = new PlayerAuctionRequest(mApplication.DB.Auction);
                mCursors.TryAdd(characterID, request);
                return request;
            }
        }

        public Hashtable GetCurrentItems(string characterID, bool reset, Hashtable filters) {
            var auction = GetOrCreatePlayerAuctionRequest(characterID);
            List<AuctionFilter> newFilters = AuctionFilter.GetList(filters);
            if(reset) {
                auction.Reset();
            }

            auction.SetFilters(newFilters);

            var pageItems = auction.CurPage();
            Hashtable result = new Hashtable {
                { (int)SPC.Index, auction.index },
                { (int)SPC.Count, auction.count }
            };
            object[] itemArr = new object[pageItems.Count];
            for(int i = 0; i < itemArr.Length; i++) {
                itemArr[i] = pageItems[i].GetInfo();
            }
            result.Add((int)SPC.Items, itemArr);
            log.InfoFormat("return item count = {0} filters = {1}", itemArr.Length, newFilters.Count);

            return result;
        }

        public Hashtable GetNextItems(string characterID ) {
            var auction = GetOrCreatePlayerAuctionRequest(characterID);
            var pageItems = auction.NextPage();
            Hashtable result = new Hashtable {
                { (int)SPC.Index, auction.index },
                { (int)SPC.Count, auction.count }
            };
            object[] itemArr = new object[pageItems.Count];
            for (int i = 0; i < itemArr.Length; i++) {
                itemArr[i] = pageItems[i].GetInfo();
            }
            result.Add((int)SPC.Items, itemArr);
            return result;
        }

        public Hashtable GetPrevItems(string characterID ) {
            var auction = GetOrCreatePlayerAuctionRequest(characterID);
            var pageItems = auction.PrevPage();
            Hashtable result = new Hashtable {
                { (int)SPC.Index, auction.index },
                { (int)SPC.Count, auction.count }
            };
            object[] itemArr = new object[pageItems.Count];
            for (int i = 0; i < itemArr.Length; i++) {
                itemArr[i] = pageItems[i].GetInfo();
            }
            result.Add((int)SPC.Items, itemArr);
            return result;
        }

        public void OnDisconnect(string characterID) {
            log.InfoFormat("character = {0} disconnect, remove auction entry", characterID);
            PlayerAuctionRequest request;

            if(string.IsNullOrEmpty(characterID)) {
                return;
            }
            mCursors.TryRemove(characterID, out request);
        }
    }
}
