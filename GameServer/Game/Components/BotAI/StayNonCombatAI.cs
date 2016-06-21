using Nebula.Server;
using Nebula.Server.Nebula.Server.Components.AI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Game.Components.BotAI {
    public class StayNonCombatAI : BaseAI, IDatabaseObject {

        private StayAINonCombatComponentData m_Data;

        public void Init(StayAINonCombatComponentData data) {
            m_Data = data;
            SetAlignWithForwardDirection(data.alignWithForwardDirection);
            SetRotationSpeed(data.rotationSpeed);
            SetAIType(new NoneAIType { battleMovingType = AttackMovingType.AttackStay });
        }

        public Hashtable GetDatabaseSave() {
            if(m_Data != null ) {
                return m_Data.AsHash();
            }
            return new Hashtable();
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
