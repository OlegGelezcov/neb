﻿using Common;
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
        }

        public byte botSubType { get {
                return mBotSubType;
            } private set {
                mBotSubType = value;
            }
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
    }
}