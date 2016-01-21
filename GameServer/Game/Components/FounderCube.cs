using Common;
using Nebula.Engine;
using Nebula.Server.Components;
using ServerClientCommon;
using Space.Game;
using System.Collections;

namespace Nebula.Game.Components {
    public class FounderCube : NebulaBehaviour, IDatabaseObject {

        private const float LIFE_TIME = 2592000;

        private FounderCubeComponentData m_Data;
        private bool m_Destroyed = false;
        private float m_PropsTimer = 0.0f;


        public void Init(FounderCubeComponentData iNData) {
            SetData(iNData);
        }

        public override void Update(float deltaTime) {
            if (false == m_Destroyed) {
                base.Update(deltaTime);
                if (m_Data == null) {
                    m_Data = new FounderCubeComponentData();
                }
                m_Data.timer += deltaTime;
                m_PropsTimer += deltaTime;
                if(m_PropsTimer >= 10.0f ) {
                    m_PropsTimer = 0f;
                    props.SetProperty((byte)SPC.Timer, m_Data.timer);
                }
                if (m_Data.timer >= LIFE_TIME) {
                    m_Destroyed = true;
                    (nebulaObject as GameObject).Destroy();
                }
            }
        }

        public override int behaviourId {
            get {
                return (int)ComponentID.FounderCube;
            }
        }

        private void SetData(FounderCubeComponentData data) {
            m_Data = data;
            if(m_Data == null ) {
                m_Data = new FounderCubeComponentData();
            }

            nebulaObject.properties.SetProperty((byte)PS.CharacterID, m_Data.characterId);
            nebulaObject.properties.SetProperty((byte)PS.CharacterName, m_Data.characterName);
            nebulaObject.properties.SetProperty((byte)PS.GuildName, m_Data.guildName);
            nebulaObject.properties.SetProperty((byte)PS.OwnerId, m_Data.gameRef);
            nebulaObject.properties.SetProperty((byte)PS.GuildId, m_Data.guildId);
            nebulaObject.properties.SetProperty((byte)PS.Timer, m_Data.timer);
        }

        public Hashtable GetDatabaseSave() {
            if(m_Data == null ) {
                m_Data = new FounderCubeComponentData();
            }
            return m_Data.AsHash();
        }
    }
}
