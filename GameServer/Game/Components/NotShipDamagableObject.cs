// NotShipDamagableObject.cs
// Nebula
//
// Created by Oleg Zheleztsov on Sunday, September 20, 2015 6:57:32 PM
// Copyright (c) 2015 KomarGames. All rights reserved.
//
using Common;
using ExitGames.Logging;
using Nebula.Engine;
using Nebula.Server.Components;
using Space.Game;

namespace Nebula.Game.Components {
    public class NotShipDamagableObject : DamagableObject {

        private static ILogger log = LogManager.GetCurrentClassLogger();

        private float mMaximumHealth;
        private PlayerBonuses mBonuses;
        private BotObject mBot;
        private EventedObject mEventedObject;

        public void Init(NotShipDamagableComponentData data) {
            SetMaximumHealth(data.maxHealth);
            SetHealth(maximumHealth);
            SetIgnoreDamageAtStart(data.ignoreDamageAtStart);
            SetIgnoreDamageInterval(data.ignoreDamageInterval);
            SetCreateChestOnKilling(data.createChestOnKilling);
        }

        public override void Start() {
            base.Start();
            mBonuses = GetComponent<PlayerBonuses>();
            SetHealth(maximumHealth);
            mBot = GetComponent<BotObject>();
            mEventedObject = GetComponent<EventedObject>();
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

        protected virtual float ModifyDamage(float damage) {
            return damage;
        }

        public override float ReceiveDamage(byte damagerType, string damagerID, float damage, byte workshop, int level, byte race) {
            float damageFromBase = base.ReceiveDamage(damagerType, damagerID, damage, workshop, level, race);
            if (!nebulaObject ) {
                return 0f;
            }
            nebulaObject.SendMessage(ComponentMessages.InCombat);

            if (ignoreDamageAtStart) {
                if(isFortification) {
                    log.InfoFormat("fortification ignored damage at start [blue]");
                }
                return 0.0f;
            }
            if (god) {
                log.InfoFormat("[{0}]: Bot if GOD, damage ignored [blue]", (nebulaObject.world as MmoWorld).Zone.Id);
                return 0f;
            }

            if(mBonuses) {
                if(mBonuses.isImmuneToDamage) {
                    log.InfoFormat("Has bonus to ignore damage... [blue]");
                    damageFromBase = 0f;
                }
            }
            float modifiedDamage = ModifyDamage(damageFromBase);
            float absorbedDamage = AbsorbDamage(modifiedDamage);

            SetHealth(health - absorbedDamage);
            AddDamager(damagerID, damagerType, absorbedDamage, workshop, level, race);

            if (mEventedObject != null) {
                mEventedObject.ReceiveDamage(new DamageInfo(damagerID, damagerType, absorbedDamage, workshop, level, race));
            }

            if(health <= 0f ) {
                SetWasKilled(true);
                nebulaObject.SendMessage(ComponentMessages.OnWasKilled);
            }

            return absorbedDamage;
        }

        public override void Death() {
            base.Death();
            (nebulaObject as GameObject).Destroy();
        }
    }
}
