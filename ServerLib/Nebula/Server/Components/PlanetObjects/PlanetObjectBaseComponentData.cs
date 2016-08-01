using Common;
using Nebula.Server.Components;
using ServerClientCommon;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Nebula.Server.Nebula.Server.Components.PlanetObjects {
    public abstract class PlanetObjectBaseComponentData : MultiComponentData, IDatabaseComponentData {

        public int row { get; private set; }
        public int column { get; private set; }
        public PlanetBasedObjectType objectType { get; private set; }
        public string ownerId { get; private set; } = string.Empty;
        public float life { get; private set; }
        public float lifeTimer { get; private set; }
        public string characterId { get; private set; } = string.Empty;
        public string characterName { get; private set; } = string.Empty;
        public string coalitionName { get; private set; } = string.Empty;

        public PlanetObjectBaseComponentData(XElement element ) {
            row = element.GetInt("row");
            column = element.GetInt("column");
            objectType = (PlanetBasedObjectType)Enum.Parse(typeof(PlanetBasedObjectType), element.GetString("object_type"));
            ownerId = element.GetString("owner");
            life = element.GetFloat("life");
            lifeTimer = 0f;
        }

        public PlanetObjectBaseComponentData(Hashtable hash ) {
            row = hash.GetValue<int>((int)SPC.Row, 0);
            column = hash.GetValue<int>((int)SPC.Column, 0);
            objectType = (PlanetBasedObjectType)hash.GetValue<int>((int)SPC.PlanetObjectType, (int)PlanetBasedObjectType.CommanderCenter);
            if(hash.ContainsKey((int)SPC.OwnerGameRef)) {
                ownerId = (string)hash.GetValue<string>((int)SPC.OwnerGameRef, string.Empty);
            }
            if(hash.ContainsKey((int)SPC.Interval)) {
                life = hash.GetValue<float>((int)SPC.Interval, 0f);
            }
            if(hash.ContainsKey((int)SPC.Timer)) {
                lifeTimer = hash.GetValue<float>((int)SPC.Timer, 0f);
            }
            if(hash.ContainsKey((int)SPC.CharacterId)) {
                this.characterId = hash.GetValue<string>((int)SPC.CharacterId, string.Empty);
            }
            if(hash.ContainsKey((int)SPC.CharacterName)) {
                this.characterName = hash.GetValue<string>((int)SPC.CharacterName, string.Empty);
            }
            if(hash.ContainsKey((int)SPC.GuildName)) {
                this.coalitionName = hash.GetValue<string>((int)SPC.GuildName, string.Empty);
            }
        }

        public PlanetObjectBaseComponentData(int row, int column, PlanetBasedObjectType objectType, string ownerId, float life, float lifeTimer,
            string characterId, string characterName, string coalitionName ) {
            this.row = row;
            this.column = column;
            this.objectType = objectType;
            this.ownerId = ownerId;
            this.life = life;
            this.lifeTimer = lifeTimer;
            this.characterId = characterId;
            this.characterName = characterName;
            this.coalitionName = coalitionName;
        }

        public virtual Hashtable AsHash() {
            return new Hashtable {
                { (int)SPC.Row, row },
                { (int)SPC.Column, column },
                { (int)SPC.PlanetObjectType, (int)objectType },
                { (int)SPC.OwnerGameRef, ownerId },
                { (int)SPC.Interval, life },
                { (int)SPC.Timer, lifeTimer },
                { (int)SPC.CharacterId, characterId },
                { (int)SPC.CharacterName, characterName },
                { (int)SPC.GuildName, coalitionName }
            };
        }

        public override ComponentID componentID {
            get {
                return ComponentID.PlanetBuilding;
            }
        }

        public void AddLifeTimer(float delta) {
            lifeTimer += delta;
        }
    }
}
