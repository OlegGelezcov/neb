// AuctionRequest.cs
// Nebula
//
// Created by Oleg Zheleztsov on Monday, September 7, 2015 11:44:00 PM
// Copyright (c) 2015 KomarGames. All rights reserved.
//
using Common;
using ServerClientCommon;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Nebula.Client.Auction {
    public class AuctionRequest : IInfoParser {
        public List<AuctionItem> items { get; private set; } = new List<AuctionItem>();
        public int index { get; private set; }
        public int count { get; private set; }

        public Dictionary<byte, List<AuctionFilter>> filters { get; private set; } = new Dictionary<byte, List<AuctionFilter>>();



        /// <summary>
        /// Detect changed filters from last call ResetChanged()
        /// </summary>
        public bool filtersChanged {
            get {
                List<string> newFilters = new List<string>();
                foreach(var p in filters ) {
                    foreach(var f in p.Value) {
                        newFilters.Add(f.key);
                    }
                }
                newFilters.Sort();
                bool equal = Enumerable.SequenceEqual(initialFilters, newFilters);
                return (!equal);
            }
        }

        private List<string> initialFilters = new List<string>();

        public void ParseInfo(Hashtable info) {
            index = info.Value<int>((int)SPC.Index, 0);
            count = info.Value<int>((int)SPC.Count, 0);
            object[] itemArr = info.Value<object[]>((int)SPC.Items, new object[] { });
            items.Clear();
            foreach(var objItem in itemArr) {
                items.Add(new AuctionItem(objItem as Hashtable));
            }
        }

        public void SetFilter(AuctionFilter filter) {
            //if already hav filters such type
            if (filters.ContainsKey((byte)filter.filterType)) {

                List<AuctionFilter> typedFilters = null;
                //get typed filters list
                if(filters.TryGetValue((byte)filter.filterType, out typedFilters)) {

                    //try find in typed list filter with same key
                    var existingFilter = typedFilters.Find(f => f.key == filter.key);

                    //if not found add new filter, 
                    if(existingFilter == null) {
                        typedFilters.Add(filter);
                    } else {
                        //and if found with same key, delete old filter and add new filter
                        int index = typedFilters.FindIndex(f => f.key == filter.key);
                        if(index >= 0) {
                            typedFilters.RemoveAt(index);
                        }
                        typedFilters.Add(filter);
                    }
                }
            } else {
                filters.Add((byte)filter.filterType, new List<AuctionFilter> { filter });
            }

        }

        public void RemoveFilter(AuctionFilter filter) {
            if(filters.ContainsKey((byte)filter.filterType)) {
                List<AuctionFilter> typedFilters = null;
                if(filters.TryGetValue((byte)filter.filterType, out typedFilters)) {
                    var existing = typedFilters.Find(f => f.key == filter.key);
                    if(existing != null ) {
                        typedFilters.Remove(existing);
                    }
                }
            }
        }

        public bool RemoveFilters(AuctionFilterType filterType ) {
            return filters.Remove((byte)filterType);
        }

        public void ClearFilters() {
            filters.Clear();
        }

        public Hashtable FilterHash() {
            Hashtable filterHash = new Hashtable();
            foreach(var pair in filters) {
                foreach(var pf in pair.Value ) {
                    if (!filterHash.ContainsKey(pf.key)) {
                        filterHash.Add(pf.key, pf.GetInfo());
                    }
                }
                
            }
            return filterHash;
        }

        /// <summary>
        /// Set initial filters for current filters
        /// </summary>
        public void ResetChanged() {

            initialFilters.Clear();
            foreach(var p in filters) {
                foreach(var f in p.Value ) {
                    initialFilters.Add(f.key);
                }
            }
            initialFilters.Sort();
        }


    }
}
