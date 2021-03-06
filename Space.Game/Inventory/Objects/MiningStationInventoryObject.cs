﻿using Common;
using ServerClientCommon;
using Space.Game.Inventory;
using System.Collections;
using System;

namespace Nebula.Inventory.Objects {
    public class MiningStationInventoryObject : IInventoryObject {

        private Hashtable mRaw;
        public int speed { get; private set; }
        public int capacity { get; private set; }
        public int race { get; private set; }
        private bool m_IsNew;

        public MiningStationInventoryObject(Hashtable info) {
            ParseInfo(info);
        }

        public MiningStationInventoryObject(int inRace, int inSpeed, int inCapacity,  bool inBinded = false ) {
            Id = "mining_station_" + inRace.ToString() + inSpeed.ToString() + inCapacity.ToString();
            race = inRace;
            speed = inSpeed;
            capacity = inCapacity;
            binded = inBinded;
            m_IsNew = true;
        }

        public bool binded {
            get;
            private set;
        }

        public string Id {
            get;
            private set;
        }

        public int Level {
            get {
                return 1;
            }
        }

        public int placingType {
            get {
                return (int)PlacingType.Inventory;
            }
        }

        public Hashtable rawHash {
            get {
                mRaw = GetInfo();
                return mRaw;
            }
        }

        public InventoryObjectType Type {
            get {
                return InventoryObjectType.mining_station;
            }
        }

        public bool splittable {
            get {
                return true;
            }
        }

        public void Bind() {
            binded = true;
        }

        public bool isNew {
            get {
                return m_IsNew;
            }
        }

        public void ResetNew() {
            m_IsNew = false;
        }
        public void SetNew(bool val) {
            m_IsNew = val;
        }
        public Hashtable GetInfo() {
            return new Hashtable {
                { (int)SPC.Id, Id },
                { (int)SPC.Level, Level },
                { (int)SPC.Color, (int)(byte)ObjectColor.white },
                { (int)SPC.ItemType, (int)(byte)Type },
                { (int)SPC.PlacingType, placingType },
                { (int)SPC.Binded, binded },
                { (int)SPC.Race, race },
                { (int)SPC.Speed, speed },
                { (int)SPC.Capacity, capacity },
                { (int)SPC.Splittable, splittable },
                { (int)SPC.IsNew, isNew }
            };
        }

        public void ParseInfo(Hashtable info) {
            mRaw = info;
            Id = info.GetValue<string>((int)SPC.Id, string.Empty);
            binded = info.GetValue<bool>((int)SPC.Binded, false);
            race = info.GetValue<int>((int)SPC.Race, (int)(byte)Race.None);
            speed = info.GetValue<int>((int)SPC.Speed, 0);
            capacity = info.GetValue<int>((int)SPC.Capacity, 0);
            m_IsNew = info.GetValue<bool>((int)SPC.IsNew, false);
        }
    }
}
