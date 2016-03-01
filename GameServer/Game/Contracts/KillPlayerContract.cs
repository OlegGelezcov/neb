using Common;
using Nebula.Game.Components;
using Nebula.Game.Events;
using ServerClientCommon;
using System.Collections;

namespace Nebula.Game.Contracts {
    public class KillPlayerContract : BaseContract {
        private int m_Count;
        private int m_Counter;

        public override void ParseInfo(Hashtable info) {
            base.ParseInfo(info);
            m_Count = info.GetValue<int>((int)SPC.Count, 0);
            m_Counter = info.GetValue<int>((int)SPC.Counter, 0);
        }

        public override string ToString() {
            string baseString =  base.ToString();
            string addedString = string.Format("count: {0}, counter: {1}", count, counter);
            return baseString + System.Environment.NewLine + addedString;
        }

        public override Hashtable GetInfo() {
            Hashtable hash = base.GetInfo();
            hash.Add((int)SPC.Count, count);
            hash.Add((int)SPC.Counter, counter);
            return hash;
        }

        public KillPlayerContract(Hashtable hash, ContractManager manager) : base(hash, manager) {
            m_Count = hash.GetValue<int>((int)SPC.Count, 0);
            m_Counter = hash.GetValue<int>((int)SPC.Counter, 0);
        }

        public KillPlayerContract(string id, int stage, string sourceWorld, ContractManager owner, int count)
            : base(id, stage, sourceWorld, ContractCategory.killPlayer, owner) {
            m_Count = count;
            m_Counter = 0;
        }

        private int count {
            get {
                return m_Count;
            }
        }

        private int counter {
            get {
                return m_Counter;
            }
        }

        private bool EventValidType(BaseEvent evt) {
            if(evt.eventType == EventType.PlayerKilled) {
                if(evt.source != null ) {
                    return true;
                }
            }
            return false;
        }

        public override ContractCheckStatus CheckEvent(BaseEvent evt) {
            if(EventValidType(evt)) {
                if(state == ContractState.accepted) {
                    if(evt.source.GetComponent<DamagableObject>().HasDamager(contractOwner.nebulaObject.Id)) {
                        m_Counter++;
                        if(counter >= count) {
                            if(Ready()) {
                                return ContractCheckStatus.ready;
                            }
                        } else {
                            SetStage(counter);
                            return ContractCheckStatus.stage_changed;
                        }
                    }
                }
            }
            return ContractCheckStatus.none;
        }
    }
}
