using Common;
using Nebula.Game.Events;
using ServerClientCommon;
using System.Collections;

namespace Nebula.Game.Contracts {
    public class WorldCaptureContract : BaseContract {

        private Race m_WorldRace;

        public WorldCaptureContract(Hashtable hash, ContractManager mgr)
            : base(hash, mgr) {
            FromHash(hash);
        }

        public WorldCaptureContract(string id, int stage, string sourceWorld, ContractManager mgr, Race wRace) 
            : base(id, stage, sourceWorld, ContractCategory.worldCapture, mgr) {
            m_WorldRace = wRace;
        }

        private void FromHash(Hashtable hash) {
            m_WorldRace = (Race)(byte)hash.GetValue<int>((int)SPC.Race, (int)Race.Humans);
        }

        public override void ParseInfo(Hashtable info) {
            base.ParseInfo(info);
            FromHash(info);
        }

        public override Hashtable GetInfo() {
            var hash = base.GetInfo();
            hash.Add((int)SPC.Race, (int)worldRace);
            return hash;
        }

        private Race worldRace {
            get {
                return m_WorldRace;
            }
        }

        public override ContractCheckStatus CheckEvent(BaseEvent evt) {
            if(evt.eventType == EventType.WorldCaptured) {
                var wcEvt = evt as WorldCapturedEvent;
                if(state == ContractState.accepted ) {
                    if(wcEvt != null ) {
                        if(wcEvt.prevRace == worldRace ) {
                            if(Ready()) {
                                return ContractCheckStatus.ready;
                            }
                        }
                    }
                }
            }
            return ContractCheckStatus.none;
        }
    }
}
