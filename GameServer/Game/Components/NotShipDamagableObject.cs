// NotShipDamagableObject.cs
// Nebula
//
// Created by Oleg Zheleztsov on Sunday, September 20, 2015 6:57:32 PM
// Copyright (c) 2015 KomarGames. All rights reserved.
//
using System;
using System.Collections;
using Common;
using ExitGames.Logging;
using Nebula.Engine;
using Nebula.Server.Components;
using Space.Game;
using Nebula.Drop;

namespace Nebula.Game.Components {
    public class NotShipDamagableObject : DamagableObject, IDatabaseObject {

        private static ILogger log = LogManager.GetCurrentClassLogger();

        private float mMaximumHealth;
        private PlayerBonuses mBonuses;
        private BotObject mBot;
        //private EventedObject mEventedObject;

        private NotShipDamagableComponentData m_InitData;

        public void Init(NotShipDamagableComponentData data) {
            m_InitData = data;
            SetMaximumHealth(data.maxHealth);
            ForceSetHealth(maximumHealth);
            SetIgnoreDamageAtStart(data.ignoreDamageAtStart);
            SetIgnoreDamageInterval(data.ignoreDamageInterval);
            SetCreateChestOnKilling(data.createChestOnKilling);
        }

        public override void Start() {
            base.Start();
            mBonuses = GetComponent<PlayerBonuses>();
            ForceSetHealth(maximumHealth);
            mBot = GetComponent<BotObject>();
            //mEventedObject = GetComponent<EventedObject>();
        }

        private bool isFortification {
            get {
                return (mBot != null) && (mBot.botSubType == (byte)BotItemSubType.Outpost);
            }
        }

        public void SetMaximumHealth(float mh) {
            mMaximumHealth = mh;
        }

        public override float maximumHealth {
            get {
                return baseMaximumHealth;
            }
        }

        public override float baseMaximumHealth {
            get {
                return mMaximumHealth;
            }
        }

        public override void Update(float deltaTime) {
            base.Update(deltaTime);
        }

        protected virtual void ModifyDamage(InputDamage damage) {
            
        }

        public override InputDamage ReceiveDamage(InputDamage inputDamage) {
            InputDamage damageFromBase = base.ReceiveDamage(inputDamage);
            if (!nebulaObject ) {
                damageFromBase.ClearAllDamages();
                //damageFromBase.SetDamage(0f);
                return damageFromBase;
            }
            nebulaObject.SendMessage(ComponentMessages.InCombat);

            if (ignoreDamageAtStart) {
                if(isFortification) {
                    log.InfoFormat("fortification ignored damage at start [blue]");
                }
                //damageFromBase.SetDamage(0f);
                damageFromBase.ClearAllDamages();
                return damageFromBase;
            }
            if (god) {
                log.InfoFormat("[{0}]: Bot if GOD, damage ignored [blue]", (nebulaObject.world as MmoWorld).Zone.Id);
                //damageFromBase.SetDamage(0f);
                damageFromBase.ClearAllDamages();
                return damageFromBase;
            }

            if(mBonuses) {
                if(mBonuses.isImmuneToDamage) {
                    log.InfoFormat("Has bonus to ignore damage... [blue]");
                    //damageFromBase.SetDamage(0.0f);
                    damageFromBase.ClearAllDamages();
                }
            }

            ModifyDamage(damageFromBase);
            AbsorbDamage(damageFromBase);

            SubHealth(damageFromBase.totalDamage);

            if (damageFromBase.hasDamager) {
                AddDamager(
                    damageFromBase.sourceId, 
                    damageFromBase.sourceType, 
                    damageFromBase.totalDamage, 
                    (byte)damageFromBase.workshop, 
                    damageFromBase.level, 
                    (byte)damageFromBase.race,
                    damageFromBase.source);
            }

            //if (mEventedObject != null) {
            //    mEventedObject.ReceiveDamage(new DamageInfo(damageFromBase.sourceId, damageFromBase.sourceType, damageFromBase.damage, (byte)damageFromBase.workshop, damageFromBase.level, (byte)damageFromBase.race));
            //}

            if(health <= 0f ) {
                SetWasKilled(true);
                nebulaObject.SendMessage(ComponentMessages.OnWasKilled);
            }



            return damageFromBase;
        }

        public override void Death() {
            base.Death();
            (nebulaObject as GameObject).Destroy();
        }

        public virtual Hashtable GetDatabaseSave() {
            if(m_InitData != null ) {
                return m_InitData.AsHash();
            }
            return new Hashtable();
        }
    }
}
