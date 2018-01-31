using Common;
using Nebula.Engine;
using Nebula.Game.Events;
using Nebula.Server.Components;

namespace Nebula.Game.Contracts {
    public class ContractObject : NebulaBehaviour {

        private ContractObjectComponentData m_Data;

        public override int behaviourId {
            get {
                return (int)ComponentID.ContractObject;
            }
        }

        public void Init(ContractObjectComponentData data ) {
            m_Data = data;
        }

        public void Death() {
            if(ContainsEventType(EventType.GameObjectDeath)) {
                ContractEvent contractEvent = new ContractEvent(contractId, EventType.GameObjectDeath, nebulaObject);
                nebulaObject.mmoWorld().OnEvent(contractEvent);
            }
        }

        private bool ContainsEventType(EventType eventType) {
            return Data?.eventTypes?.Contains(eventType) ?? false;
        }

        private string contractId {
            get {
                if(m_Data != null && (false == string.IsNullOrEmpty(m_Data.contractId))) {
                    return m_Data.contractId;
                }
                return string.Empty;
            }
        }

        private ContractObjectComponentData Data => m_Data;
    }
}
