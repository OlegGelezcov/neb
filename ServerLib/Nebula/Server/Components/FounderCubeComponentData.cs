using Common;
using GameMath;
using Nebula.Server.Components;
using ServerClientCommon;
using System.Collections;

namespace Nebula.Server.Components {
    public class FounderCubeComponentData : ComponentData, IDatabaseComponentData {

        private string m_CharacterId = string.Empty;
        private string m_GuildId = string.Empty;
        private string m_CharacterName = string.Empty;
        private string m_GameRef = string.Empty;
        private string m_GuildName = string.Empty;
        private float m_Timer = 0f;

        public float timer {
            get {
                return m_Timer;
            }
            set {
                m_Timer = Mathf.ClampLess(value, 0);
            }
        }
        public string characterId {
            get {
                if(m_CharacterId == null ) {
                    m_CharacterId = string.Empty;
                }
                return m_CharacterId;
            }
            set {
                if(value != null ) {
                    m_CharacterId = value;
                } else {
                    m_CharacterId = string.Empty;
                }
            }
        }
        public string guildId {
            get {
                if(m_GuildId == null ) {
                    m_GuildId = string.Empty;
                }
                return m_GuildId;
            }
            set {
                if(value != null ) {
                    m_GuildId = value;
                } else {
                    m_GuildId = string.Empty;
                }
            }
        }

        public string characterName {
            get {
                if(m_CharacterName == null) {
                    m_CharacterName = string.Empty;
                }
                return m_CharacterName;
            }
            set {
                if(value != null ) {
                    m_CharacterName = value;
                } else {
                    m_CharacterName = string.Empty;
                }
            }
        }

        public string gameRef {
            get {
                if(m_GameRef == null ) {
                    m_GameRef = string.Empty;
                }
                return m_GameRef;
            }
            set {
                m_GameRef = (value == null) ? string.Empty : value;
            }
        }

        public string guildName {
            get {
                if(m_GuildName == null) {
                    m_GuildName = string.Empty;
                }
                return m_GuildName;
            }
            set {
                m_GuildName = (value != null) ? value : string.Empty;
            }
        }

        public override ComponentID componentID {
            get {
                return ComponentID.FounderCube;
            }
        }

        public FounderCubeComponentData() : 
            this (string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, 0f) {

        }

        public FounderCubeComponentData(string inCharacterId, string inCharacterName, string inGuildId, string inGuildName, string inGameRef, float inTimer) {
            characterId = inCharacterId;
            characterName = inCharacterName;
            guildId = inGuildId;
            guildName = inGuildName;
            gameRef = inGameRef;
            timer = inTimer;
        }

        public FounderCubeComponentData(Hashtable hash) {
            characterId     = hash.GetValue<string>((int)SPC.CharacterId,       string.Empty);
            characterName   = hash.GetValue<string>((int)SPC.CharacterName,     string.Empty);
            guildId         = hash.GetValue<string>((int)SPC.Guild,             string.Empty);
            guildName       = hash.GetValue<string>((int)SPC.GuildName,         string.Empty);
            gameRef         = hash.GetValue<string>((int)SPC.GameRefId,         string.Empty);
            timer = hash.GetValue<float>((int)SPC.Timer, 0f);
        }

        public Hashtable AsHash() {
            return new Hashtable {
                {(int)SPC.CharacterId, characterId },
                {(int)SPC.CharacterName, characterName },
                {(int)SPC.Guild, guildId },
                {(int)SPC.GuildName, guildName },
                {(int)SPC.GameRefId, gameRef },
                {(int)SPC.Timer, timer }
            };
        }
    }
}
