using Common;
using Nebula.Engine;
using Nebula.Server.Nebula.Server.Components.PlanetObjects;
using ServerClientCommon;
using Space.Game;
using System;
using System.Collections;

namespace Nebula.Game.Components.PlanetObjects {
    public abstract class PlanetObjectBase : NebulaBehaviour, IDatabaseObject {
        public int row { get; private set; }
        public int column { get; private set; }
        public string ownerId { get; private set; }
        public abstract PlanetBasedObjectType objectType { get; }
        protected PlanetObjectBaseComponentData m_Data;
        private bool m_BindedToWorld = false;

        private bool m_Destroyed = false;
        private float m_PropTimer = 0;
        private float m_LastReceiveDamageNotificationTime = 0.0f;

        public override void Start() {
            BindToWorld();
        }

        public virtual void Init(PlanetObjectBaseComponentData data ) {
            m_Data = data;
            if(data != null ) {
                row = data.row;
                column = data.column;
                ownerId = data.ownerId;
            }
            BindToWorld();

            props.SetProperty((byte)PS.Info, GetBuildingProperties());
        }

        public override void Update(float deltaTime) {
            base.Update(deltaTime);
            if(m_Data != null ) {
                m_Data.AddLifeTimer(deltaTime);
                if(m_Data.lifeTimer >= m_Data.life ) {
                    DestroySelf();
                }

                m_PropTimer += deltaTime;
                if (m_PropTimer >= 10.0f) {
                    m_PropTimer = 0f;
                    props.SetProperty((byte)PS.Info, GetBuildingProperties());
                }
            }
        }

        private void DestroySelf() {
            if(!m_Destroyed ) {
                m_Destroyed = true;
                (nebulaObject as GameObject).Destroy();
            }
        }

        public virtual Hashtable GetBuildingProperties() {
            Hashtable hash = new Hashtable {
                { (int)SPC.Row, row },
                { (int)SPC.Column, column },
                { (int)SPC.PlanetObjectType, (int)objectType },
                { (int)SPC.OwnerGameRef, ownerId },
               
            };
            if(m_Data != null ) {
                hash.Add((int)SPC.Interval, m_Data.life);
                hash.Add((int)SPC.Timer, m_Data.lifeTimer);
                hash.Add((int)SPC.CharacterId, m_Data.characterId);
                hash.Add((int)SPC.CharacterName, m_Data.characterName);
                hash.Add((int)SPC.GuildName, m_Data.coalitionName);
            }
            return hash;
        }

        private void BindToWorld() {
            if(!m_BindedToWorld) {
                if(m_Data != null ) {
                    if(nebulaObject != null ) {
                        if(nebulaObject.mmoWorld() != null ) {
                            nebulaObject.mmoWorld().SetCellObject(row, column, this);
                            m_BindedToWorld = true;
                        }
                    }
                }
            }
        }

        public override int behaviourId {
            get {
                return (int)ComponentID.PlanetBuilding;
            }
        }

        public virtual Hashtable GetDatabaseSave() {
            if(m_Data != null ) {
                return m_Data.AsHash();
            }
            return new Hashtable();
        }

        public void Death() {
            try {
                var world = nebulaObject.mmoWorld();
                if(world.IsObjectAtCell(row, column, nebulaObject.Id)) {
                    nebulaObject.mmoWorld().UnsetCellObject(row, column);
                }
                
            } catch(Exception exception) {

            }
        }

        public virtual void OnNewDamage(DamageInfo damager) {
            if ((Time.curtime() - m_LastReceiveDamageNotificationTime) >= MiningStation.RECEIVE_DAMAGE_NOTIFICATION_INTERVAL) {
                m_LastReceiveDamageNotificationTime = Time.curtime();
                if (m_Data != null && (!string.IsNullOrEmpty(m_Data.characterId))) {
                    nebulaObject.mmoWorld().application.updater.CallS2SMethod(
                        NebulaCommon.ServerType.SelectCharacter,
                        "PlanetObjectUnderAttackNotification",
                        new object[] { m_Data.characterId, nebulaObject.mmoWorld().Zone.Id, damager.race, (int)objectType });
                }
            }
        }
    }
}
