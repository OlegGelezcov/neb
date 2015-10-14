using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using System.Collections;
using Space.Game.Inventory;
using ServerClientCommon;

namespace Space.Game
{
    public class WorkhouseStation : IInfo
    {
        //private ServerWorkhouseStationHold hold;
        private ServerInventory stationInventory;


        public WorkhouseStation() : this(null) { }

        public WorkhouseStation(Hashtable info )
        {
            //this.hold = new ServerWorkhouseStationHold(100);
            this.stationInventory = new ServerInventory(100);

            if (info != null )
            {
                this.ParseInfo(info);
            }
        }

        //public IWorkhouseStationHold Hold
        //{
        //    get
        //    {
        //        return this.hold;
        //    }
        //}

        public ServerInventory StationInventory
        {
            get
            {
                return this.stationInventory;
            }
        }

        public Hashtable GetInfo()
        {
            Hashtable result = new Hashtable();

            //result.Add((int)SPC.Hold,       this.hold.GetInfo());
            result.Add((int)SPC.Inventory,  this.stationInventory.GetInfo());

            return result;
        }


        public void ParseInfo(Hashtable info)
        {
            //this.hold = new ServerWorkhouseStationHold(info.GetValue<Hashtable>((int)SPC.Hold, new Hashtable()));
            this.stationInventory = new ServerInventory(info.GetValue<Hashtable>((int)SPC.Inventory, new Hashtable()));
        }

        //public void SetHold(Hashtable holdInfo )
        //{
        //    this.hold = new ServerWorkhouseStationHold(holdInfo);
        //}

        public void SetInventory(Hashtable inventoryInfo)
        {
            this.stationInventory = new ServerInventory(inventoryInfo);
        }

        public void SetInventory(ServerInventory sInventory)
        {
            this.stationInventory = sInventory;
        }
    }
}
