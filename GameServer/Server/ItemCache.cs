// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ItemCache.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   A cache for <see cref="Item">items</see>. Each <see cref="IWorld" /> has one item cache.
//   It uses an <see cref="ReaderWriterLockSlim" /> to ensure thread safety.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Space.Server
{
    using ExitGames.Threading;
    using Space.Game;
    using GameMath;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Common;
    using Nebula.Engine;
    using System.Collections.Concurrent;


    public class ItemCache {
        private readonly ConcurrentDictionary<byte, ItemCacheL2> itemCaches;

        public readonly int maxLockMilliseconds;

        public ItemCache(int lockMilliseconds) {
            maxLockMilliseconds = lockMilliseconds;
            itemCaches = new ConcurrentDictionary<byte, ItemCacheL2>();
        }

        public List<NebulaObject> Filter(Func<NebulaObject, bool> filter) {
            List<NebulaObject> result = new List<NebulaObject>();
            foreach(var cache2 in itemCaches) {
                foreach(var itPair in cache2.Value.GetItems()) {
                    if(filter(itPair.Value)) {
                        result.Add(itPair.Value);
                    }
                }
            }
            return result;
        }

        public bool Contains(byte type, string id) {
            if(itemCaches.ContainsKey(type)) {
                return itemCaches[type].Contains(id);
            }
            return false;
        }

        public void Tick(float deltaTime) {
            foreach(var cachePair in itemCaches) {
                cachePair.Value.Tick(deltaTime);
            }
        }

        public void SendMessage(string message, object arg = null ) {
            foreach(var p in itemCaches) {
                p.Value.SendMessage(message, arg);
            }
        }

        public bool AddItem(Item item) {
            ItemCacheL2 level2Cache = GetLevel2Cache(item.Type);
            return level2Cache.AddItem(item);
        }

        public bool RemoveItem(byte itemType, string itemId) {
            ItemCacheL2 level2Cache = this.GetLevel2Cache(itemType);
            return level2Cache.RemoveItem(itemId);
        }

        public Dictionary<string, Item> GetItems(byte type) {
            ItemCacheL2 level2Cache = GetLevel2Cache(type);
            string[] ids = null;
            Dictionary<string, Item> result = new Dictionary<string, Item>();
            if(level2Cache.TryGetIds(out ids)) {
                if(ids != null) {
                    for(int i = 0; i < ids.Length; i++) {
                        Item it;
                        if(level2Cache.TryGetItem(ids[i], out it)) {
                            result.Add(ids[i], it);
                        }
                    }
                }
            }
            return result;
        }

        public Dictionary<string, Item> GetItems(byte type, Func<Item, bool> predicate) {
            var its = GetItems(type);
            Dictionary<string, Item> filteredItems = new Dictionary<string, Item>();
            foreach(var pit in its) {
                if(predicate(pit.Value)) {
                    filteredItems.Add(pit.Key, pit.Value);
                }
            }
            return filteredItems;
        }

        public ConcurrentDictionary<string, Item> GetItemsConcurrent(byte type, Func<Item, bool> predicate) {
            var its = GetItems(type);
            ConcurrentDictionary<string, Item> filteredItems = new ConcurrentDictionary<string, Item>();
            foreach(var pit in its) {
                if(predicate(pit.Value)) {
                    filteredItems.TryAdd(pit.Key, pit.Value);
                }
            }
            return filteredItems;
        }

        public ConcurrentDictionary<string, Item> GetItems(Func<Item, bool> filter) {
            ConcurrentDictionary<string, Item> result = new ConcurrentDictionary<string, Item>();
            foreach(var itCache in itemCaches) {
                foreach(var it in itCache.Value.GetItems(filter)) {
                    result.TryAdd(it.Key, it.Value);
                }
            }
            return result;
        }

        public Item GetItem(Func<Item, bool> filter) {
            foreach(var itCache in itemCaches) {
                var it = itCache.Value.GetItem(filter);
                if(it) {
                    return it;
                }
            }
            return null;
        }

        public void DeleteItems() {
            foreach(var itCache in itemCaches) {
                itCache.Value.DeleteItems();
            }
            itemCaches.Clear();
        }


        public bool TryGetItem(byte itemType, string itemId, out Item item) {
            ItemCacheL2 level2Cache = this.GetLevel2Cache(itemType);
            return level2Cache.TryGetItem(itemId, out item);
        }

        public bool TryGetItem(byte itemType, Vector3 center, float radius, Func<Item, bool> customFilter, out Item item) {
            ItemCacheL2 level2Cache = this.GetLevel2Cache(itemType);
            return level2Cache.TryGetItem(center, radius, customFilter, out item);
        }

        public bool TryGetIds(byte itemType, out string[] ids) {
            ItemCacheL2 level2Cache = this.GetLevel2Cache(itemType);
            return level2Cache.TryGetIds(out ids);
        }

        public int TotalCount() {
            int count = 0;
            foreach (var val in Enum.GetValues(typeof(ItemType))) {
                byte type = (byte)(ItemType)val;
                count += this.GetCount(type, it => true);
            }
            return count;
        }


        public int GetCount(byte itemType, System.Func<Item, bool> predicate) {
            ItemCacheL2 level2Cache = this.GetLevel2Cache(itemType);
            return level2Cache.Count(predicate);
        }

        public int GetCount(byte itemType) {
            ItemCacheL2 level2Cache = this.GetLevel2Cache(itemType);
            return level2Cache.count;
        }


        public ItemCacheL2 GetLevel2Cache(byte itemType) {
            ItemCacheL2 result;
            if(itemCaches.TryGetValue(itemType, out result)) {
                return result;
            }

            if(!itemCaches.TryGetValue(itemType, out result)) {
                result = new ItemCacheL2(maxLockMilliseconds);
                itemCaches.TryAdd(itemType, result);
            }
            return result;
        }


        public class ItemCacheL2 {
            private readonly ConcurrentDictionary<string, Item> items;
            public ItemCacheL2(int lockMilliseconds) {
                items = new ConcurrentDictionary<string, Item>();
            }

            public void DeleteItems() {
                foreach(var pItem in items) {
                    pItem.Value.Destroy();
                }
                items.Clear();
            }

            public int count {
                get {
                    return items.Count;
                }
            }

            public bool Contains(string id) {
                return items.ContainsKey(id);
            }

            public void Tick(float deltaTime) {
                foreach(var pair in items) {
                    pair.Value.Update(deltaTime);
                }
            }

            public void SendMessage(string message, object arg = null ) {
                foreach(var p in items) {
                    if(p.Value) {
                        p.Value.SendMessage(message, arg);
                    }
                }
            }

            public bool AddItem(Item item) {
                if(items.ContainsKey(item.Id)) {
                    return false;
                }
                return items.TryAdd(item.Id, item);
            }

            public bool RemoveItem(string itemId) {
                Item i;
                return items.TryRemove(itemId, out i);
            }

            public bool TryGetItem(string itemId, out Item item) {
                return items.TryGetValue(itemId, out item);
            }

            public int Count(Func<Item, bool> predicate) {
                int counter = 0;
                foreach(var pair in items) {
                    if (predicate(pair.Value))
                        counter++;
                }
                return counter;
            }

            public int Count() {
                return items.Count;
            }

            public bool TryGetIds(out string[] ids) {
                List<string> listIds = new List<string>();
                foreach(var p in items) {
                    listIds.Add(p.Key);
                }
                ids = listIds.ToArray();
                return true;
            }

            public bool TryGetItem(Vector3 center, float radius, Func<Item, bool> filter, out Item item) {
                item = null;
                List<Item> filtered = new List<Item>();
                foreach(var it in items) {
                    if(Vector3.Distance(center, it.Value.transform.position) < radius) {
                        if(filter(it.Value)) {
                            filtered.Add(it.Value);
                        }
                    }
                }

                if(filtered.Count > 0 ) {
                    item = filtered[Rand.Int() % filtered.Count];
                    return true;
                }
                return false;
            }

            public ConcurrentDictionary<string, Item> GetItems() {
                return items;
            }

            public ConcurrentDictionary<string, Item> GetItems(Func<Item, bool> filter) {
                ConcurrentDictionary<string, Item> reult = new ConcurrentDictionary<string, Item>();
                foreach(var it in items) {
                    if(it.Value) {
                        if(filter(it.Value)) {
                            reult.TryAdd(it.Key, it.Value);
                        }
                    }
                }
                return reult;
            }

            public Item GetItem(Func<Item, bool> filter) {
                foreach(var it in items) {
                    if(it.Value) {
                        if(filter(it.Value)) {
                            return it.Value;
                        }
                    }
                }
                return null;
            }
        }
    }

    ///// <summary>
    /////   A cache for <see cref = "Item">items</see>. Each <see cref = "IWorld" /> has one item cache.
    /////   It uses an <see cref = "ReaderWriterLockSlim" /> to ensure thread safety.
    ///// </summary>
    ///// <remarks>
    /////   All members are thread safe.
    ///// </remarks>
    //public class ItemCache : IDisposable
    //{
    //    #region Constants and Fields

    //    /// <summary>
    //    ///   The item caches.
    //    /// </summary>
    //    private readonly Dictionary<byte, ItemCacheL2> itemCaches;

    //    /// <summary>
    //    ///   The max lock milliseconds.
    //    /// </summary>
    //    public readonly int maxLockMilliseconds;

    //    /// <summary>
    //    ///   The reader writer lock.
    //    /// </summary>
    //    public readonly ReaderWriterLockSlim readerWriterLock;
    //    private readonly object lockObject = new object();

    //    #endregion

    //    #region Constructors and Destructors

    //    /// <summary>
    //    ///   Initializes a new instance of the <see cref = "ItemCache" /> class.
    //    /// </summary>
    //    /// <param name = "maxLockMilliseconds">
    //    ///   The max Lock Milliseconds.
    //    /// </param>
    //    public ItemCache(int maxLockMilliseconds)
    //    {
    //        this.maxLockMilliseconds = maxLockMilliseconds;
    //        this.itemCaches = new Dictionary<byte, ItemCacheL2>();
    //        this.readerWriterLock = new ReaderWriterLockSlim(  );
    //    }

    //    public List<NebulaObject> Filter(Func<NebulaObject, bool> filter) {
    //        List<NebulaObject> result = new List<NebulaObject>();
    //        foreach(var cache2 in itemCaches) {
    //            foreach(var itPair in cache2.Value.GetItems()) {
    //                if(itPair.Value) {
    //                    if(filter(itPair.Value)) {
    //                        result.Add(itPair.Value);
    //                    }
    //                }
    //            }
    //        }
    //        return result;
    //    } 

    //    public void Tick(float deltaTime) {
    //        foreach(var cachePair in itemCaches) {
    //            cachePair.Value.Tick(deltaTime);
    //        }
    //    }

    //    /// <summary>
    //    ///   Finalizes an instance of the <see cref = "ItemCache" /> class.
    //    /// </summary>
    //    ~ItemCache()
    //    {
    //        this.Dispose(false);
    //    }

    //    #endregion

    //    #region Public Methods

    //    /// <summary>
    //    ///   Adds an <see cref = "Item" />.
    //    /// </summary>
    //    /// <param name = "item">
    //    ///   The new item.
    //    /// </param>
    //    /// <returns>
    //    ///   true if item added.
    //    /// </returns>
    //    public bool AddItem(Item item)
    //    {
    //        ItemCacheL2 level2Cache = this.GetLevel2Cache(item.Type);
    //        return level2Cache.AddItem(item);
    //    }

    //    /// <summary>
    //    ///   Removes an <see cref = "Item" />.
    //    /// </summary>
    //    /// <param name = "itemType">
    //    ///   The item Type.
    //    /// </param>
    //    /// <param name = "itemId">
    //    ///   The item id.
    //    /// </param>
    //    /// <returns>
    //    ///   true if item removed.
    //    /// </returns>
    //    public bool RemoveItem(byte itemType, string itemId)
    //    {
    //        ItemCacheL2 level2Cache = this.GetLevel2Cache(itemType);
    //        return level2Cache.RemoveItem(itemId);
    //    }

    //    public Dictionary<string, Item> GetItems(byte type )
    //    {
    //        ItemCacheL2 level2Cache = this.GetLevel2Cache(type);

    //        string[] ids = null;
    //        Dictionary<string, Item> result = new Dictionary<string, Item>();


    //        if (level2Cache.TryGetIds(out ids))
    //        {

    //            if (ids != null)
    //            {
    //                for (int i = 0; i < ids.Length; i++)
    //                {
    //                    Item it;
    //                    if (level2Cache.TryGetItem(ids[i], out it))
    //                    {
    //                        result.Add(ids[i], it);
    //                    }
    //                }
    //            }
    //        }

    //        return result;
    //    }

    //    public Dictionary<string, Item> GetItems(byte type, Func<Item, bool> predicate)
    //    {
    //        var its = this.GetItems(type);
    //        Dictionary<string, Item> filteredItems = new Dictionary<string, Item>();
    //        foreach(var pIt in its )
    //        {
    //            if(predicate(pIt.Value))
    //            {
    //                filteredItems.Add(pIt.Key, pIt.Value);
    //            }
    //        }
    //        return filteredItems;
    //    }

    //    /// <summary>
    //    ///   Tries to retrieve an <see cref = "Item" />.
    //    /// </summary>
    //    /// <param name = "itemType">
    //    ///   The item Type.
    //    /// </param>
    //    /// <param name = "itemId">
    //    ///   The item id.
    //    /// </param>
    //    /// <param name = "item">
    //    ///   The found item.
    //    /// </param>
    //    /// <returns>
    //    ///   true if item was found.
    //    /// </returns>
    //    public bool TryGetItem(byte itemType, string itemId, out Item item)
    //    {
    //        ItemCacheL2 level2Cache = this.GetLevel2Cache(itemType);
    //        return level2Cache.TryGetItem(itemId, out item);
    //    }

    //    public bool TryGetItem(byte itemType, Vector3 center, float radius, Func<Item, bool> customFilter, out Item item) {
    //        ItemCacheL2 level2Cache = this.GetLevel2Cache(itemType);
    //        return level2Cache.TryGetItem(center, radius, customFilter, out item);
    //    }

    //    public bool TryGetIds(byte itemType, out string[] ids) {
    //        ItemCacheL2 level2Cache = this.GetLevel2Cache(itemType);
    //        return level2Cache.TryGetIds(out ids);
    //    }

    //    public int TotalCount()
    //    {
    //        int count = 0;
    //        foreach(var val in Enum.GetValues(typeof(ItemType)))
    //        {
    //            byte type = (byte)(ItemType)val;
    //            count += this.GetCount(type, it => true);
    //        }
    //        return count;
    //    }

    //    public int GetCount(byte itemType, System.Func<Item, bool> predicate )
    //    {
    //        ItemCacheL2 level2Cache = this.GetLevel2Cache(itemType);
    //        return level2Cache.Count(predicate);
    //    }

    //    #endregion

    //    #region Implemented Interfaces

    //    #region IDisposable

    //    /// <summary>
    //    ///   The dispose.
    //    /// </summary>
    //    public void Dispose()
    //    {
    //        this.Dispose(true);
    //        //GC.SuppressFinalize(this);
    //    }

    //    #endregion

    //    #endregion

    //    #region Methods

    //    /// <summary>
    //    ///   Clears the cache and disposes the rw lock.
    //    /// </summary>
    //    /// <param name = "disposing">
    //    ///   The disposing.
    //    /// </param>
    //    protected virtual void Dispose(bool disposing)
    //    {
    //        if (disposing)
    //        {
    //            this.readerWriterLock.Dispose();

    //            foreach (ItemCacheL2 level2Cache in this.itemCaches.Values)
    //            {
    //                level2Cache.Dispose();
    //            }

    //            this.itemCaches.Clear();
    //        }
    //    }

    //    /// <summary>
    //    ///   The get level 2 cache.
    //    /// </summary>
    //    /// <param name = "itemType">
    //    ///   The item type.
    //    /// </param>
    //    /// <returns>
    //    ///   the level2 cache for the item type
    //    /// </returns>
    //    public ItemCacheL2 GetLevel2Cache(byte itemType)
    //    {
    //        ItemCacheL2 result;
    //        using (ReadLock.TryEnter(this.readerWriterLock, this.maxLockMilliseconds))
    //        {
    //            if (this.itemCaches.TryGetValue(itemType, out result))
    //            {
    //                return result;
    //            }
    //        }

    //        using (WriteLock.TryEnter(this.readerWriterLock, this.maxLockMilliseconds))
    //        {
    //            if (false == this.itemCaches.TryGetValue(itemType, out result))
    //            {
    //                result = new ItemCacheL2(this.maxLockMilliseconds);
    //                this.itemCaches.Add(itemType, result);
    //            }

    //            return result;
    //        }
    //    }

    //    #endregion

    //    /// <summary>
    //    ///   The item cache l 2.
    //    /// </summary>
    //    public class ItemCacheL2 : IDisposable
    //    {
    //        #region Constants and Fields

    //        /// <summary>
    //        ///   The items.
    //        /// </summary>
    //        private readonly Dictionary<string, Item> items;

    //        /// <summary>
    //        ///   The max lock milliseconds.
    //        /// </summary>
    //        private readonly int maxLockMilliseconds;

    //        /// <summary>
    //        ///   The reader writer lock.
    //        /// </summary>
    //        private readonly ReaderWriterLockSlim readerWriterLock;

    //        #endregion

    //        #region Constructors and Destructors

    //        /// <summary>
    //        ///   Initializes a new instance of the <see cref = "ItemCache.ItemCacheL2" /> class.
    //        /// </summary>
    //        /// <param name = "maxLockMilliseconds">
    //        ///   The max Lock Milliseconds.
    //        /// </param>
    //        public ItemCacheL2(int maxLockMilliseconds)
    //        {
    //            this.maxLockMilliseconds = maxLockMilliseconds;
    //            this.items = new Dictionary<string, Item>();
    //            this.readerWriterLock = new ReaderWriterLockSlim();
    //        }

    //        //private object syncObject = new object();
    //        public void Tick(float deltaTime) {
    //            //using (WriteLock.TryEnter(readerWriterLock, Settings.MaxLockWaitTimeMilliseconds)) {
    //                foreach (var itemPair in items) {
    //                    itemPair.Value.Update(deltaTime);
    //                }
    //            //}
    //        }

    //        /// <summary>
    //        ///   Finalizes an instance of the <see cref = "ItemCache.ItemCacheL2" /> class.
    //        /// </summary>
    //        ~ItemCacheL2()
    //        {
    //            this.DisposeReaderWriterLock();
    //        }

    //        #endregion

    //        #region Public Methods

    //        /// <summary>
    //        ///   The add item.
    //        /// </summary>
    //        /// <param name = "item">
    //        ///   The new item.
    //        /// </param>
    //        /// <returns>
    //        ///   true if item added.
    //        /// </returns>
    //        public bool AddItem(Item item)
    //        {
    //            using (WriteLock.TryEnter(this.readerWriterLock, this.maxLockMilliseconds))
    //            {
    //                if (this.items.ContainsKey(item.Id))
    //                {
    //                    return false;
    //                }

    //                this.items.Add(item.Id, item);
    //                return true;
    //            }
    //        }

    //        /// <summary>
    //        ///   The remove item.
    //        /// </summary>
    //        /// <param name = "itemId">
    //        ///   The item id.
    //        /// </param>
    //        /// <returns>
    //        ///   true if item removed.
    //        /// </returns>
    //        public bool RemoveItem(string itemId)
    //        {
    //            using (WriteLock.TryEnter(this.readerWriterLock, this.maxLockMilliseconds))
    //            {
    //                return this.items.Remove(itemId);
    //            }
    //        }

    //        /// <summary>
    //        ///   The try get item.
    //        /// </summary>
    //        /// <param name = "itemId">
    //        ///   The item id.
    //        /// </param>
    //        /// <param name = "item">
    //        ///   The found item.
    //        /// </param>
    //        /// <returns>
    //        ///   true if item was found.
    //        /// </returns>
    //        public bool TryGetItem(string itemId, out Item item)
    //        {
    //            using (ReadLock.TryEnter(this.readerWriterLock, this.maxLockMilliseconds))
    //            {
    //                return this.items.TryGetValue(itemId, out item);
    //            }
    //        }

    //        public int Count()
    //        {
    //            return this.Count(it => true);
    //        }

    //        public int Count(System.Func<Item, bool> predicate )
    //        {
    //            using(ReadLock.TryEnter(this.readerWriterLock, this.maxLockMilliseconds))
    //            {
    //                int couter = 0;
    //                foreach(var pair in this.items)
    //                {
    //                    if (predicate(pair.Value))
    //                        couter++;
    //                }
    //                return couter;
    //            }
    //        }

    //        public bool TryGetIds(out string[] ids) 
    //        {
    //            List<string> listIds = new List<string>();
    //            using (ReadLock.TryEnter(this.readerWriterLock, this.maxLockMilliseconds)) {
    //                foreach (var key in this.items.Keys) {
    //                    listIds.Add(key);
    //                }
    //                ids = listIds.ToArray();
    //                return true;
    //            }
    //        }

    //        public bool TryGetItem(Vector3 center, float radius, Func<Item, bool> customFilter, out Item item) {
    //            item = null;
    //            using (ReadLock.TryEnter(this.readerWriterLock, this.maxLockMilliseconds)) {
    //                List<Item> filteredItems = items
    //                    .Where((pair) => Vector3.Distance(center, pair.Value.transform.position) < radius)
    //                    .Where(pair => customFilter(pair.Value))
    //                    .Select(pair => pair.Value).ToList();
    //                if (filteredItems.Count > 0)
    //                {
    //                    item = filteredItems[Rand.Int() % filteredItems.Count];
    //                    return true;
    //                }
    //            }
    //            return false;
    //        }

    //        public Dictionary<string, Item> GetItems()
    //        {
    //            using (ReadLock.TryEnter(this.readerWriterLock, this.maxLockMilliseconds))
    //            {
    //                return this.items;
    //            }
    //        }

    //        #endregion

    //        #region Implemented Interfaces

    //        #region IDisposable

    //        /// <summary>
    //        ///   The dispose.
    //        /// </summary>
    //        public void Dispose()
    //        {
    //            this.DisposeReaderWriterLock();
    //            //GC.SuppressFinalize(this);
    //        }

    //        #endregion

    //        #endregion

    //        #region Methods

    //        /// <summary>
    //        ///   The dispose reader writer lock.
    //        /// </summary>
    //        private void DisposeReaderWriterLock()
    //        {
    //            this.readerWriterLock.Dispose();
    //        }

    //        #endregion
    //    }
    //}
}