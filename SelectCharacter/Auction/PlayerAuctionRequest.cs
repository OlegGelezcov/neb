using Common;
using GameMath;
using MongoDB.Driver;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace SelectCharacter.Auction {
    public class PlayerAuctionRequest {

        public const int PAGE_COUNT = 20;

        private MongoCollection<AuctionItem> mCollection;

        public PlayerAuctionRequest(MongoCollection<AuctionItem> collection ) {
            mCollection = collection;
        }

        public PlayerAuctionRequest() : this( null ) { }




        public int index { get; private set; } = 0;
        public int count { get; private set; } = 0;
        public ConcurrentDictionary<AuctionFilterType, ConcurrentBag<AuctionFilter>> filters { get; private set; } = new ConcurrentDictionary<AuctionFilterType, ConcurrentBag<AuctionFilter>>();

        public void SetFilters(List<AuctionFilter> newFilters ) {
            var newKeys = newFilters.Select(f => f.key).ToList();
            List<string> oldKeys = new List<string>();
            foreach(var p in filters) {
                foreach(var f in p.Value) {
                    oldKeys.Add(f.key);
                }
            }
            bool equal = Enumerable.SequenceEqual<string>(newKeys.OrderBy(k => k), oldKeys.OrderBy(k => k));

            if(!equal) {
                Reset();
                foreach(var f in newFilters) {
                    if(filters.ContainsKey(f.filterType)) {
                        ConcurrentBag<AuctionFilter> bag = null;
                        if(filters.TryGetValue(f.filterType, out bag)) {
                            bag.Add(f);
                        }
                    } else {
                        filters.TryAdd(f.filterType, new ConcurrentBag<AuctionFilter> { f });
                    }
                }
            } else {
                filters.Clear();
                foreach (var f in newFilters) {
                    if (filters.ContainsKey(f.filterType)) {
                        ConcurrentBag<AuctionFilter> bag = null;
                        if (filters.TryGetValue(f.filterType, out bag)) {
                            bag.Add(f);
                        }
                    } else {
                        filters.TryAdd(f.filterType, new ConcurrentBag<AuctionFilter> { f });
                    }
                }
            }
        }


        public List<AuctionItem> NextPage() {
            if(mCollection == null ) { return new List<AuctionItem>(); }

            var cursor = mCollection.FindAll();
            count = cursor.Where(item => CheckFilters(item)).Count();
            index += PAGE_COUNT;
            index = Mathf.Clamp(index, 0, count);
            cursor = mCollection.FindAll();
            return cursor.Where(item => CheckFilters(item)).Skip(index).Take(PAGE_COUNT).ToList();
        }

        public List<AuctionItem> PrevPage() {
            if(mCollection == null ) { return new List<AuctionItem>(); }
            var cursor = mCollection.FindAll();
            count = cursor.Where(item => CheckFilters(item)).Count();
            index -= PAGE_COUNT;
            index = Mathf.Clamp(index, 0, count);
            cursor = mCollection.FindAll();
            return cursor.Where(item => CheckFilters(item)).Skip(index).Take(PAGE_COUNT).ToList();
        }

        public List<AuctionItem> CurPage() {
            if(mCollection == null ) { return new List<AuctionItem>();  }
            var cursor = mCollection.FindAll();
            count = cursor.Where(item => CheckFilters(item)).Count();
            index = Mathf.Clamp(index, 0, count);
            cursor = mCollection.FindAll();
            return cursor.Where(item => CheckFilters(item)).Skip(index).Take(PAGE_COUNT).ToList();
        }

        public void Reset() {
            index = 0;
            count = 0;
            filters.Clear();
        }

        private bool CheckFilters(AuctionItem item) {
            if(filters.Count == 0 ) {
                return true;
            }

            bool check = true;
            foreach(var pair in filters ) {
                if (!CheckSubFilters(pair.Value, item)) {
                    check = false;
                    break;
                }
            }
            return check;
        }

        private bool CheckSubFilters(ConcurrentBag<AuctionFilter> subFilters, AuctionItem item) {
            bool success = false;
            foreach(var f in subFilters ) {
                if(f.Check(item)) {
                    success = true;
                    break;
                }
            }
            return success;
        }

    }
}
