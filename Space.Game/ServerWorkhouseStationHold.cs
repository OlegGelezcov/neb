using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Common;
using Space.Game.Ship;
using ServerClientCommon;

namespace Space.Game
{
    //public class ServerWorkhouseStationHold : IInfo, IWorkhouseStationHold
    //{
    //    /// <summary>
    //    /// max slots in hold
    //    /// </summary>
    //    protected int maxSlots;

    //    /// <summary>
    //    /// items in hold
    //    /// </summary>
    //    protected Dictionary<StationHoldableObjectType, Dictionary<string, IStationHoldableObject>> items;

    //    public ServerWorkhouseStationHold(int maxSlots)
    //    {
    //        if (maxSlots >= 0)
    //        {
    //            this.maxSlots = maxSlots;
    //        }
    //        this.items = new Dictionary<StationHoldableObjectType, Dictionary<string, IStationHoldableObject>>();
    //    }

    //    public ServerWorkhouseStationHold(Hashtable info )
    //    {
    //        this.items = new Dictionary<StationHoldableObjectType, Dictionary<string, IStationHoldableObject>>();
    //        this.ParseInfo(info);
    //    }

    //    public int MaxSlots
    //    {
    //        get
    //        {
    //            return maxSlots;
    //        }
    //    }

    //    public Dictionary<StationHoldableObjectType, Dictionary<string, IStationHoldableObject>> Items
    //    {
    //        get
    //        {
    //            return this.items;
    //        }
    //    }

    //    public int SlotsUsed
    //    {
    //        get
    //        {
    //            return this.items.Sum(kv => kv.Value.Count);
    //        }
    //    }

    //    public bool TryGetItem(StationHoldableObjectType type, string id, out IStationHoldableObject item)
    //    {
    //        item = default(IStationHoldableObject);
    //        Dictionary<string, IStationHoldableObject> typedItems;
    //        if (this.items.TryGetValue(type, out typedItems))
    //        {
    //            return typedItems.TryGetValue(id, out item);
    //        }
    //        return false;
    //    }

    //    public bool TryGetItem(string id, out IStationHoldableObject item) {
    //        item = default(IStationHoldableObject);
    //        foreach(var typed in items) {
    //            foreach(var itemPair in typed.Value) {
    //                if(itemPair.Key == id ) {
    //                    item = itemPair.Value;
    //                    return true;
    //                }
    //            }
    //        }
    //        return false;
    //    }

    //    public bool Add(IStationHoldableObject obj)
    //    {
    //        if(this.items == null )
    //        {
    //            this.items = new Dictionary<StationHoldableObjectType, Dictionary<string, IStationHoldableObject>>();
    //        }

    //        IStationHoldableObject item = default(IStationHoldableObject);
    //        if (this.TryGetItem(obj.Type, obj.Id, out item) == false)
    //        {
    //            if (this.SlotsUsed < this.MaxSlots)
    //            {
    //                if (this.items.ContainsKey(obj.Type))
    //                {
    //                    this.items[obj.Type].Add(obj.Id, obj);
    //                }
    //                else
    //                {
    //                    this.items.Add(obj.Type, new Dictionary<string, IStationHoldableObject> { { obj.Id, obj } });
    //                }
    //                return true;
    //            }
    //        }
    //        return false;
    //    }

    //    public bool Remove(StationHoldableObjectType type, string id)
    //    {
    //        if (this.items.ContainsKey(type))
    //        {
    //            return this.items[type].Remove(id);
    //        }
    //        return false;
    //    }

    //    public int ItemCount(StationHoldableObjectType type)
    //    {
    //        if (this.items.ContainsKey(type))
    //        {
    //            return this.items[type].Count;
    //        }
    //        return 0;
    //    }

    //    public bool HasItem(StationHoldableObjectType type, string id)
    //    {
    //        IStationHoldableObject item = default(IStationHoldableObject);
    //        return this.TryGetItem(type, id, out item);
    //    }

    //    public bool HasFreeSpace() {
    //        return this.MaxSlots - this.SlotsUsed > 0;
    //    }

    //    public int ChangeMaxSlots(int newValue)
    //    {
    //        if (newValue >= 0)
    //        {
    //            int oldValue = this.maxSlots;
    //            this.maxSlots = newValue;
    //            return oldValue;
    //        }
    //        return this.maxSlots;
    //    }

    //    public void Clear()
    //    {
    //        if(this.items == null )
    //        {
    //            this.items = new Dictionary<StationHoldableObjectType, Dictionary<string, IStationHoldableObject>>();
    //        }
    //        this.items.Clear();
    //    }

    //    public void Replace(IWorkhouseStationHold newHold)
    //    {
    //        this.Clear();
    //        this.ChangeMaxSlots(newHold.MaxSlots);

    //        foreach (var pair in newHold.Items)
    //        {
    //            foreach (var p2 in pair.Value)
    //            {
    //                if (this.items.ContainsKey(pair.Key) == false)
    //                {
    //                    this.items.Add(pair.Key, new Dictionary<string, IStationHoldableObject>());
    //                }
    //                this.items[pair.Key].Add(p2.Key, p2.Value);
    //            }
    //        }
    //    }


    //    public Hashtable GetInfo()
    //    {
    //        Hashtable result = new Hashtable();
    //        Hashtable hitems = new Hashtable();
    //        result.Add((int)SPC.MaxSlots, this.maxSlots);

    //        foreach (var p1 in this.items)
    //        {
    //            foreach (var p2 in p1.Value)
    //            {
    //                hitems.Add(p2.Value.Id, p2.Value.GetInfo());
    //            }
    //        }
    //        result.Add((int)SPC.Items, hitems);

    //        return result;
    //    }

    //    public static IStationHoldableObject Create(Hashtable info ) {
    //        var type = (StationHoldableObjectType)(byte)info.Value<int>((int)SPC.HoldType, (int)StationHoldableObjectType.Module.toByte());
    //        switch(type) {
    //            case StationHoldableObjectType.Module:
    //                return new ShipModule(info);
    //            default:
    //                throw new Exception(string.Format("unsupported hold object type = {0}", type));
    //        }
    //    }


    //    public void ParseInfo(Hashtable info)
    //    {
    //        this.Clear();
    //        this.maxSlots = info.GetValue<int>((int)SPC.MaxSlots, 0);
    //        Hashtable itemsInfo = info.GetValue<Hashtable>((int)SPC.Items, new Hashtable());
    //        foreach (DictionaryEntry entry in itemsInfo)
    //        {
    //            Hashtable itInfo = entry.Value as Hashtable;
    //            if (itInfo != null)
    //            {
    //                StationHoldableObjectType holdType = (StationHoldableObjectType)itInfo.GetValue<int>((int)SPC.HoldType, (int)StationHoldableObjectType.Module.toByte());
    //                switch (holdType)
    //                {
    //                    case StationHoldableObjectType.Module:
    //                        {
    //                            this.Add(new ShipModule(itInfo));
    //                        }
    //                        break;
    //                    default:
    //                        throw new Exception("unknow element");
    //                }
    //            }
    //        }
    //    }
    //}
}
