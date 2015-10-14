//using Common;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Collections;
//using ServerClientCommon;

//namespace Nebula.Client
//{
//    public class ClientWorkhouseStationHold : IInfoParser
//    {

//        private int maxSlots;
//        private Hashtable items;

//        public ClientWorkhouseStationHold(int MaxSlots) {
//            maxSlots = MaxSlots;

//            items = new Hashtable();
//        }

//        public ClientWorkhouseStationHold(Hashtable info) {
//            items = new Hashtable();
//            ParseInfo(info);
//        }

//        public int MaxSlots {
//            get {
//                return maxSlots;
//            }
//        }

//        public Hashtable Items {
//            get {
//                return items;
//            }
//        }

//        public List<IStationHoldableObject> OrderedItems {
//            get {
//                List<StationHoldableObjectType> types = new List<StationHoldableObjectType>();
//                foreach (DictionaryEntry entry in items) {
//                    types.Add((StationHoldableObjectType)entry.Key);
//                }
//                types.Sort();

//                List<IStationHoldableObject> result = new List<IStationHoldableObject>();

//                foreach (StationHoldableObjectType type in types) {
//                    List<string> ids = new List<string>();
//                    Hashtable typedItems = items[type] as Hashtable;
//                    foreach (DictionaryEntry entry in typedItems) {
//                        ids.Add(entry.Key.ToString());
//                    }
//                    ids.Sort();
//                    foreach (string id in ids) {
//                        result.Add(typedItems[id] as IStationHoldableObject);
//                    }
//                }
//                return result;
//            }
//        }

//        public int SlotsUsed {
//            get {
//                int count = 0;
//                foreach (DictionaryEntry entry in items) {
//                    Hashtable typedItems = entry.Value as Hashtable;
//                    count += typedItems.Count;
//                }
//                return count;
//            }
//        }

//        public bool TryGetItem(StationHoldableObjectType type, string id, out IStationHoldableObject item) {
//            item = null;
//            Hashtable typedItems;
//            if (items.ContainsKey(type)) {
//                typedItems = items[type] as Hashtable;
//                if (typedItems.ContainsKey(id)) {
//                    item = typedItems[id] as IStationHoldableObject;
//                    return true;
//                }
//            }
//            return false;
//        }

//        public bool Add(IStationHoldableObject obj) {
//            if (items == null) {
//                items = new Hashtable();
//            }
//            IStationHoldableObject item = null;
//            if (false == TryGetItem(obj.Type, obj.Id, out item)) {
//                if (SlotsUsed < MaxSlots) {
//                    if (items.ContainsKey(obj.Type)) {
//                        (items[obj.Type] as Hashtable).Add(obj.Id, obj);
//                    } else {
//                        items.Add(obj.Type, new Hashtable { { obj.Id, obj } });
//                    }
//                    return true;
//                }
//            }
//            return false;
//        }

//        public bool Remove(StationHoldableObjectType type, string id) {
//            if (items.ContainsKey(type)) {
//                Hashtable typedItems = items[type] as Hashtable;
//                if (typedItems.ContainsKey(id)) {
//                    typedItems.Remove(id);
//                    return true;
//                }
//            }
//            return false;
//        }

//        public int ItemCount(StationHoldableObjectType type) {
//            if (items.ContainsKey(type)) {
//                var typedItems = items[type] as Hashtable;
//                return typedItems.Count;
//            }
//            return 0;
//        }

//        public bool HasItem(StationHoldableObjectType type, string id) {
//            IStationHoldableObject item = null;
//            return TryGetItem(type, id, out item);
//        }

//        public int ChangeMaxSlots(int newValue) {
//            if (newValue > 0) {
//                int oldValue = maxSlots;
//                maxSlots = newValue;
//                return oldValue;
//            }
//            return maxSlots;
//        }

//        public void Clear() {
//            if (items == null) {
//                items = new Hashtable();
//            }
//            items.Clear();
//        }

//        public void Replace(ClientWorkhouseStationHold newHold) {
//            Clear();
//            ChangeMaxSlots(newHold.MaxSlots);
//            foreach (DictionaryEntry pair in newHold.Items) {
//                Hashtable typedItems = pair.Value as Hashtable;
//                foreach (DictionaryEntry pair2 in typedItems) {
//                    if (!items.ContainsKey(pair.Key)) {
//                        items.Add(pair.Key, new Hashtable { });
//                    }
//                    (items[pair.Key] as Hashtable).Add(pair2.Key, pair2.Value);
//                }
//            }
//        }



//        public void ParseInfo(Hashtable info) {
//            Clear();
//            this.maxSlots = info.GetValue<int>((int)SPC.MaxSlots, 0);
//            Hashtable itemsInfo = info.GetValue<Hashtable>((int)SPC.Items, new Hashtable());
//            foreach (DictionaryEntry entry in itemsInfo) {
//                Hashtable itInfo = entry.Value as Hashtable;
//                if (itInfo == null) {
//                    continue;
//                }
//                StationHoldableObjectType holdType = (StationHoldableObjectType)itInfo.GetValue<byte>((int)SPC.HoldType, StationHoldableObjectType.Module.toByte());
//                switch (holdType) {
//                    case StationHoldableObjectType.Module:
//                        {
//                            Add(new ClientShipModule(itInfo));
//                            break;
//                        }
//                    default:
//                        {
//                            throw new Exception("unknow element");
//                        }
//                }
//            }
//        }

//        public bool HasFreeSpace() {
//            return (MaxSlots - SlotsUsed > 0);
//        }

//        public bool TryGetObjects(StationHoldableObjectType type, out Hashtable typedItems) {
//            typedItems = null;
//            if (items.ContainsKey(type)) {
//                typedItems = items[type] as Hashtable;
//                return true;
//            }
//            return false;
//        }
//    }
//}
