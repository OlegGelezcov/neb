using Common;
using Nebula.Engine;
using Nebula.Server.Components;

namespace Nebula.Game.Contracts.Marks {
    public abstract class ContractMark : NebulaBehaviour {

        private ContractMarkData m_Data;

        public virtual void Init(ContractMarkData data) {
            m_Data = data;
            if (props != null) {
                UpdateProperty();
            }
        }

        public override void Start() {
            base.Start();
            UpdateProperty();
        }

        private void UpdateProperty() {
            if (m_Data != null) {
                props.SetProperty((byte)PS.ContractId, m_Data.contractId);
            } else {
                props.SetProperty((byte)PS.ContractId, string.Empty);
            }
        }

        public override int behaviourId {
            get {
                return (int)ComponentID.ContractMark;
            }
        }
    }
}
