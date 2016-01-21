using Nebula.Server.Components;
using Nebula.Server;
using ExitGames.Logging;
using System;
using System.Collections;

namespace Nebula.Game.Components.BotAI {
    public class StayCombatAI : CombatBaseAI, IDatabaseObject {

        private static ILogger log = LogManager.GetCurrentClassLogger();

        private StayAIComponentData m_Data;

        public Hashtable GetDatabaseSave() {
            if(m_Data != null ) {
                return m_Data.AsHash();
            }
            return new Hashtable();
        }

        public void Init(StayAIComponentData data) {
            m_Data = data;

            base.Init(data);

            SetAIType(new NoneAIType { battleMovingType = data.battleMovingType });
            log.InfoFormat("StayCombatAI.Init(): batlle type = {0}", data.battleMovingType);
        }

        public override void Start() {
            base.Start();
            Move(transform.position, transform.position, transform.position, transform.rotation, 0);
        }

        public override void Update(float deltaTime) {
            base.Update(deltaTime);
        }

        protected override void OnDoIdle(float deltatTime) {
            
        }
    }
}
