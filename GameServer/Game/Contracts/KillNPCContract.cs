﻿using Common;
using Nebula.Game.Events;
using System.Collections;

namespace Nebula.Game.Contracts {
    public class KillNPCContract : BaseContract {

        public override Hashtable GetInfo() {
            return base.GetInfo();
        }

        public override void ParseInfo(Hashtable info) {
            base.ParseInfo(info);
        }

        public KillNPCContract(Hashtable hash) 
            : base(hash) { }

        public KillNPCContract(string id, ContractState state, int stage, string sourceWorld)
            : base(id, state, stage, sourceWorld, ContractCategory.killNPC) { }

        public override ContractCheckStatus CheckEvent(BaseEvent evt) {
            if ((evt.eventType == EventType.GameObjectDeath) && (state == ContractState.accepted)) {
                if (evt is ContractEvent) {
                    ContractEvent contractEvent = evt as ContractEvent;
                    if(contractEvent.contractId == id ) {
                        if (SetState(ContractState.ready)) {
                            return ContractCheckStatus.ready;
                        }
                    }
                }
            }
            return ContractCheckStatus.none;
        }
    }
}
