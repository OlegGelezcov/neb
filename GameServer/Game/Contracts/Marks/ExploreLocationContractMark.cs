﻿using Common;
using Nebula.Server.Components;

namespace Nebula.Game.Contracts.Marks {
    public class ExploreLocationContractMark : ContractMark {

        private ExploreLocationContractMarkData m_Data;

        public override  void Init(ContractMarkData data) {
            base.Init(data);
            m_Data = data as ExploreLocationContractMarkData;
            if(props != null ) {
                UpdateProperty();
            }
        }

        public override void Start() {
            base.Start();
            UpdateProperty();
        }

        private void UpdateProperty() {
            if(m_Data != null ) {
                props.SetProperty((byte)PS.Mark, m_Data.locationName);
            } else {
                props.SetProperty((byte)PS.Mark, string.Empty);
            }
        }
    }
}
