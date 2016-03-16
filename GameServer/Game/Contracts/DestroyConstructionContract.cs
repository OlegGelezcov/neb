using Common;
using Nebula.Game.Components;
using Nebula.Game.Events;
using ServerClientCommon;
using System;
using System.Collections;

namespace Nebula.Game.Contracts {
    public class DestroyConstructionContract : BaseContract {

        private BotItemSubType m_ConstructionType;
        private Race m_ConstructionRace;


        public DestroyConstructionContract(Hashtable hash, ContractManager mgr) : base(hash, mgr) {
            FromHash(hash);
        }

        public DestroyConstructionContract(string id, int stage, string sourceWorld, ContractManager mgr, BotItemSubType constrType, Race constrRace) 
            : base(id, stage, sourceWorld, ContractCategory.destroyConstruction, mgr) {
            m_ConstructionType = constrType;
            m_ConstructionRace = constrRace;
        }

        public override void ParseInfo(Hashtable info) {
            base.ParseInfo(info);
            FromHash(info);
        }

        private void FromHash(Hashtable hash) {
            m_ConstructionType = (BotItemSubType)(byte)hash.GetValue<int>((int)SPC.SubType, (int)BotItemSubType.Turret);
            m_ConstructionRace = (Race)(byte)hash.GetValue<int>((int)SPC.Race, (int)Race.Borguzands);
        }
        public override string ToString() {
            string bString = base.ToString();
            string nString = string.Format("construction type = {0}, construction race = {1}", constructionType, constructionRace);
            return bString + Environment.NewLine + nString;
        }

        public override Hashtable GetInfo() {
            var hash = base.GetInfo();
            hash.Add((int)SPC.SubType, (int)constructionType);
            hash.Add((int)SPC.Race, (int)constructionRace);
            return hash;
        }

        private BotItemSubType constructionType {
            get {
                return m_ConstructionType;
            }
        }
        private Race constructionRace {
            get {
                return m_ConstructionRace;
            }
        }

        private bool EventValidType(BaseEvent evt) {
            if(evt.eventType == EventType.ConstructionKilled) {
                if(evt.source != null ) {
                    return true;
                }
            }
            return false;
        }

        private bool ConstructionValidType(BaseEvent evt) {
            var botObject = evt.source.GetComponent<BotObject>();
            var raceableObject = evt.source.GetComponent<RaceableObject>();
            if(botObject != null && raceableObject != null ) {
                if(botObject.botSubType == (byte)constructionType && raceableObject.race == (byte)constructionRace ) {
                    return true;
                }
            }
            return false;
        }

        public override ContractCheckStatus CheckEvent(BaseEvent evt) {
            if(EventValidType(evt)) {
                if (ConstructionValidType(evt)) {
                    if (state == ContractState.accepted) {
                        if (evt.source.GetComponent<DamagableObject>().HasDamager(contractOwner.nebulaObject.Id)) {
                            if (Ready()) {
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
