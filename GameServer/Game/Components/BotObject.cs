using Common;
using Nebula.Engine;
using Nebula.Server.Components;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Collections;

namespace Nebula.Game.Components {
    public class BotObject : NebulaBehaviour, IDatabaseObject {

        private byte mBotSubType = byte.MaxValue;

        private BotComponentData mInitData;

        public void Init(BotComponentData data) {
            mInitData = data;
            SetSubType(data.subType);
            UpdateBotGroupProperty();
        }

        public bool isConstruction {
            get {
                return (mBotSubType == (byte)BotItemSubType.Outpost) ||
                    (mBotSubType == (byte)BotItemSubType.MainOutpost) ||
                    (mBotSubType == (byte)BotItemSubType.Turret);
            }
        }

        private void UpdateBotGroupProperty() {
            if (props != null && mInitData != null) {
                if (mInitData.botGroup != null) {
                    props.SetProperty((byte)PS.BotGroup, mInitData.botGroup);
                } else {
                    props.SetProperty((byte)PS.BotGroup, string.Empty);
                }
            }
        }

        public byte botSubType { get {
                return mBotSubType;
            } private set {
                mBotSubType = value;
            }
        }

        public BotItemSubType getSubType() {
            return (BotItemSubType)botSubType;
        }

        public void SetSubType(byte subType) {
            botSubType = subType;
        }

        public void SetSubType(BotItemSubType subType) {
            SetSubType((byte)subType);
        }

        public override void Update(float deltaTime) {
            props.SetProperty((byte)PS.SubType, botSubType);
        }

        public override void Start() {
            if(mBotSubType == byte.MaxValue) {
                throw new Exception("botSubType not setted");
            }
            //set at properties group of bot
            //if(mInitData != null ) {
            //    props.SetProperty((int)PS.BotGroup, (mInitData.botGroup != null) ? mInitData.botGroup : string.Empty);
            //}
            UpdateBotGroupProperty();
        }

        public Hashtable GetDatabaseSave() {
            if(mInitData != null ) {
                return mInitData.AsHash();
            }
            return new Hashtable();
        }

        public override int behaviourId {
            get {
                return (int)ComponentID.Bot;
            }
        }

        public bool isTurret {
            get {
                return (mBotSubType == (byte)BotItemSubType.Turret);
            }
        }

        public string botGroup {
            get {
                if(mInitData!= null ) {
                    if(mInitData.botGroup != null ) {
                        return mInitData.botGroup;
                    }
                }
                return string.Empty;
            }
        }

        public bool HasDamager(string id) {
            var damagable = GetComponent<DamagableObject>();
            if(damagable == null ) {
                return false;
            }
            return damagable.HasDamager(id);
        }
    }
}
