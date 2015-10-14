// DrillSchemeObjectInfo.cs
// Nebula
//
// Created by Oleg Zheleztsov on Friday, February 6, 2015 3:02:45 PM
// Copyright (c) 2015 KomarGames. All rights reserved.
//
namespace Nebula.Client.Inventory.Objects
{
    using Common;
    using ServerClientCommon;
    using System;
    using System.Collections;

    public class DrillSchemeObjectInfo : IInventoryObjectInfo
    {
        private string id;
        private int level;
        private string template;
        private int numSlots;
        private float productionSpeed;
        private float protectionInterval;
        private float health;
        public bool binded { get; private set; }

        public DrillSchemeObjectInfo(Hashtable info)
        {
            this.ParseInfo(info);
        }

        public string Id
        {
            get { return this.id; }
        }

        public InventoryObjectType Type
        {
            get { return InventoryObjectType.DrillScheme; }
        }

        public int Level
        {
            get
            {
                return this.level;
            }
        }

        public string Template
        {
            get
            {
                return this.template;
            }
        }

        public int NumSlots
        {
            get
            {
                return this.numSlots;
            }
        }

        public float ProductionSpeed
        {
            get
            {
                return this.productionSpeed;
            }
        }

        public float ProtectionInterval
        {
            get
            {
                return this.protectionInterval;
            }
        }

        public float Health
        {
            get
            {
                return this.health;
            }
        }

        public int placingType {
            get {
                return (int)PlacingType.Inventory;
            }
        }

        public Hashtable rawHash {
            get;
            private set;
        }

        public Hashtable GetInfo()
        {
            return new Hashtable
            {
                {(int)SPC.Id, this.Id },
                {(int)SPC.Level, this.Level },
                {(int)SPC.Template, this.Template },
                {(int)SPC.NumSlots, this.NumSlots },
                {(int)SPC.ProductionSpeed, this.ProductionSpeed },
                {(int)SPC.ProtectionInterval, this.ProtectionInterval },
                {(int)SPC.Health, this.Health },
                {(int)SPC.Binded, binded }
            };
        }

        public void ParseInfo(Hashtable info)
        {
            rawHash = info;
            this.id = info.GetValue<string>((int)SPC.Id, string.Empty);
            this.level = info.GetValue<int>((int)SPC.Level, 0);
            this.template = info.GetValue<string>((int)SPC.Template, string.Empty);
            this.numSlots = info.GetValue<int>((int)SPC.NumSlots, 0);
            this.productionSpeed = info.GetValue<float>((int)SPC.ProductionSpeed, 0.0f);
            this.protectionInterval = info.GetValue<float>((int)SPC.ProtectionInterval, 0.0f);
            this.health = info.GetValue<float>((int)SPC.Health, 0.0f);
            binded = info.GetValue<bool>((int)SPC.Binded, false);
        }

        public bool HasColor() {
            return false;
        }

        public ObjectColor MyColor() {
            return ObjectColor.white;
        }
    }
}
