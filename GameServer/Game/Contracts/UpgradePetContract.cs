using Nebula.Game.Events;
using System.Collections;

namespace Nebula.Game.Contracts {
    public class UpgradePetContract : BaseContract {
        public UpgradePetContract(Hashtable hash, ContractManager mgr)
            : base(hash, mgr) { }

        public UpgradePetContract(string id, int stage, string sourceWorld, ContractManager mgr)
            : base(id, stage, sourceWorld, Common.ContractCategory.upgradeCompanion, mgr) { }

        private bool EventValidType(BaseEvent evt) {
            if(evt.eventType == Common.EventType.UpgradePet ) {
                if(evt.source != null ) {
                    return true;
                }
            }
            return false;
        }

        public override ContractCheckStatus CheckEvent(BaseEvent evt) {
            if(EventValidType(evt)) {
                if(state == Common.ContractState.accepted ) {
                    if(Ready()) {
                        return ContractCheckStatus.ready;
                    }
                }
            }
            return ContractCheckStatus.none;
        }
    }
}
