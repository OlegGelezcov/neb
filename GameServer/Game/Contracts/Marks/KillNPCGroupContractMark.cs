using Common;
using Nebula.Server.Components;

namespace Nebula.Game.Contracts.Marks {
    public class KillNPCGroupContractMark : ContractMark {

        private KillNPCGroupContractMarkData m_Data;

        public void Init(KillNPCGroupContractMarkData data ) {
            base.Init(data);
            m_Data = data;
            if(props != null ) {
                UpdateProperty();
            }
        }

        public override void Start() {
            base.Start();
            UpdateProperty();
        }

        private void UpdateProperty() {
            if (m_Data != null) {
                props.SetProperty((byte)PS.BotGroup, m_Data.group);
            } else {
                props.SetProperty((byte)PS.BotGroup, string.Empty);
            }
        }
    }
}
