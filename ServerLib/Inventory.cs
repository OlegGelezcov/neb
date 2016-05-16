using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Common
{
    public class Inventory<T, U> 
        where T : InventoryItem<U>, new ()
        where U : IInventoryObjectBase
    {
        protected int _maxSlots;

        protected ConcurrentDictionary<InventoryObjectType, ConcurrentDictionary<string, T>> _items;
        
        //protected Dictionary<InventoryObjectType, Dictionary<string, T>> _items;

        public Inventory()
        {
            _maxSlots = 0;
            _items = new ConcurrentDictionary<InventoryObjectType, ConcurrentDictionary<string, T>>();
        }
        public Inventory(int maxSlots) {
            _maxSlots = maxSlots;
            _items = new ConcurrentDictionary<InventoryObjectType, ConcurrentDictionary<string, T>>();
        }

        public int MaxSlots
        {
            get
            {
                return _maxSlots;
            }
        }

        public void ResetNew() {
            foreach(var pFiltered in _items) {
                foreach(var pItem in pFiltered.Value) {
                    pItem.Value.Object.ResetNew();
                }
            }
        }

        /// <summary>
        /// Return all items dictionary in inventory
        /// </summary>
        public ConcurrentDictionary<InventoryObjectType, ConcurrentDictionary<string, T>> Items {
            get {
                return _items;
            }
        }

        public int SlotsUsed
        {
            get
            {
                return _items.Sum((pair) => pair.Value.Count);
            }
        }

        public bool TryGetItem(InventoryObjectType type, string id, out T item) {
            item = default(T);
            ConcurrentDictionary<string, T> typedItems;
            if (_items.TryGetValue(type, out typedItems)) {
                return typedItems.TryGetValue(id, out item);
            }
            return false;
        }

        public bool TryGetItem(string id, out T item) {
            item = default(T);
            foreach(var typedItems in _items) {
                if(typedItems.Value.TryGetValue(id, out item)) {
                    return true;
                }
            }
            return false;
        }

        public virtual bool Add(U obj, int count) {
            T item = default(T);
            if (TryGetItem(obj.Type, obj.Id, out item)) {
                item.Add(count);
                item.Object.SetNew(true);
                return true;
            }
            else
            {
                if (SlotsUsed < MaxSlots) {
                    if (_items.ContainsKey(obj.Type))
                    {
                        T nItem = new T();
                        nItem.Set(obj, count);
                        _items[obj.Type].TryAdd(obj.Id, nItem);
                        nItem.Object.SetNew(true);
                    }
                    else {
                        T nItem = new T();
                        nItem.Set(obj, count);
                        nItem.Object.SetNew(true);

                        var ncd = new ConcurrentDictionary<string, T>();
                        if (!string.IsNullOrEmpty(obj.Id)) {
                            ncd.TryAdd(obj.Id, nItem);
                        }
                        _items.TryAdd(obj.Type, ncd);
                    }
                    return true;
                }
            }
            return false;
        }


        public void Remove(InventoryObjectType type, string id, int count) {
            T item = default(T);
            if (TryGetItem(type, id, out item)) {
                item.Remove(count);
                item.Object.ResetNew();
                if (false == item.Has) {
                    T oldVal;
                    _items[type].TryRemove(item.Object.Id, out oldVal);
                }
            }
        }

        public int ItemCount(InventoryObjectType type, string id)
        {
            T item = default(T);
            if (TryGetItem(type, id, out item)) {
                return item.Count;
            }
            return 0;
        }

        public int ItemCount(string id) {
            T item = default(T);
            foreach(var p in Items) {
                if ( p.Value.ContainsKey(id) ) {
                    item = p.Value[id];
                    return item.Count;
                }
            }
            return 0;
        }

        public bool HasItem(InventoryObjectType type, string id) {
            T item = default(T);
            if (TryGetItem(type, id, out item)) {
                return item.Has;
            }
            return false;
        }

        public bool HasItem(string id ) {
            T item = default(T);
            if(TryGetItem(id, out item)) {
                return item.Has;
            }
            return false;
        }

        public override string ToString()
        {
            System.Text.StringBuilder sb = new StringBuilder();
            foreach (var pair in _items) {
                sb.AppendLine(pair.Key.ToString());
                foreach (var pair2 in pair.Value) {
                    sb.AppendFormat("    {0}, count: {1}\n", pair2.Key, pair2.Value.Count);
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Change max slots number in inventory in return old value of max slots
        /// </summary>
        public int ChangeMaxSlots(int newValue) {
            int oldValue = _maxSlots;
            _maxSlots = newValue;
            return oldValue;
        }

        /// <summary>
        /// Remove all items from inventory
        /// </summary>
        public void Clear()
        {
            _items.Clear();
        }

        /// <summary>
        /// Replace all inventory with content new inventory, max slots changed as newInventory
        /// </summary>
        public void Replace(Inventory<T, U> newInventory) {
            Clear();

            ChangeMaxSlots(newInventory.MaxSlots);

            foreach (var pair in newInventory.Items) {
                foreach (var pair2 in pair.Value) {
                    if (_items.ContainsKey(pair.Key) == false)
                    {
                        _items.TryAdd(pair.Key, new ConcurrentDictionary<string, T>());
                    }
                    _items[pair.Key].TryAdd(pair2.Key, pair2.Value);
                }
            }
        }

        /// <summary>
        /// Replace item with new item if already in inventory, or add to inventory if not exist yet in inventory
        /// </summary>
        /// <param name="item"></param>
        public void ReplaceItem(T item) {
            //add typed items if needed
            if (_items.ContainsKey(item.Object.Type) == false) {
                _items.TryAdd(item.Object.Type, new ConcurrentDictionary<string, T>());
            }

            //find typed items
            ConcurrentDictionary<string, T> typedItems = _items[item.Object.Type];

            //if item already present replace it, else add as new item
            if (typedItems.ContainsKey(item.Object.Id))
            {
                typedItems[item.Object.Id].Set(item.Object, item.Count);
            }
            else 
            {
                typedItems.TryAdd(item.Object.Id, item);
            }
        }
    }
}
