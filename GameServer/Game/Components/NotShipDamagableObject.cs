// NotShipDamagableObject.cs
// Nebula
//
// Created by Oleg Zheleztsov on Sunday, September 20, 2015 6:57:32 PM
// Copyright (c) 2015 KomarGames. All rights reserved.
//
using ExitGames.Logging;
using Nebula.Engine;
using Nebula.Server.Components;
using Space.Game;

namespace Nebula.Game.Components {
    public class NotShipDamagableObject : DamagableObject {

        private static ILogger log = LogManager.GetCurrentClassLogger();

        private float mMaximumHealth;
        private PlayerBonuses mBonuses;

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
            base.ReceiveDamage(damagerType, damagerID, damage, workshop, level, race);
            if (!nebulaObject ) { return 0f; }

            nebulaObject.SendMessage(ComponentMessages.InCombat);

            if (ignoreDamageAtStart) {
                //log.InfoFormat("Damage ignored at start time to end = {0}", mIgnoreDamageTimer);
                return 0;
            }
            if (god) {
                log.InfoFormat("[{0}]: Bot if GOD, damage ifnored", (nebulaObject.world as MmoWorld).Zone.Id);
                return 0f;
            }

            if(mBonuses) {
                if(mBonuses.isImmuneToDamage) {
                    log.InfoFormat("Has bonus to ignore damaga.");
                    damage = 0f;
                }
            }

            //if(damage > 0f) {
            //    log.InfoFormat("object = {0} receive damage = {1}", nebulaObject.Id, damage);
            //}
            damage = ModifyDamage(damage);
            damage = AbsorbDamage(damage);

            SetHealth(health - damage);
            AddDamager(damagerID, damagerType, damage, workshop, level, race);
            var eventedObject = GetComponent<EventedObject>();
            if (eventedObject) {
                eventedObject.ReceiveDamage(new DamageInfo(damagerID, damagerType, damage, workshop, level, race));
            }

            if(health <= 0f ) {
                SetWasKilled(true);
            }

            return damage;
        }

        public override void Death() {
            base.Death();
            (nebulaObject as GameObject).Destroy();
        }
    }
}
