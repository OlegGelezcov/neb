using Common;
using Nebula.Game.Components;
using Nebula.Game.Events;
using ServerClientCommon;
using System;
using System.Collections;

namespace Nebula.Game.Contracts {
    public class KillNPCGroupContract : BaseContract  {

        private int m_Count;
        private string m_BotGroupName;
        private int m_Counter;
        private string m_TargetWorld;

        public override void ParseInfo(Hashtable info) {
            base.ParseInfo(info);
            m_Count = info.GetValue<int>((int)SPC.Count, 0);
            m_BotGroupName = info.GetValue<string>((int)SPC.Group, string.Empty);
            m_Counter = info.GetValue<int>((int)SPC.Counter, 0);
            m_TargetWorld = info.GetValue<string>((int)SPC.TargetWorld, string.Empty);
        }

        public override string ToString() {
            string baseString =  base.ToString();
            string addedString = string.Format("count: {0}, group name: {1}, counter: {2}, target world: {3}",
                count, botGroupName, counter, targetWorld);
            return baseString + Environment.NewLine + addedString;
        }

        public override Hashtable GetInfo() {
            Hashtable hash =  base.GetInfo();
            hash.Add((int)SPC.Count, count);
            hash.Add((int)SPC.Group, botGroupName);
            hash.Add((int)SPC.Counter, m_Counter);
            hash.Add((int)SPC.TargetWorld, m_TargetWorld);
            return hash;
        }

        public KillNPCGroupContract(Hashtable hash, ContractManager manager)
            : base(hash, manager) {
            m_Count = hash.GetValue<int>((int)SPC.Count, 0);
            m_BotGroupName = hash.GetValue<string>((int)SPC.Group, string.Empty);
            m_Counter = hash.GetValue<int>((int)SPC.Counter, 0);
            m_TargetWorld = hash.GetValue<string>((int)SPC.TargetWorld, string.Empty);
        }

        public KillNPCGroupContract(string id,  int stage, string sourceWorld, ContractManager owner, int cnt, string grpName, string targetWorld)
            : base(id,  stage, sourceWorld, ContractCategory.killNPCGroup, owner) {
            m_Count = cnt;
            m_BotGroupName = grpName;
            m_Counter = 0;
            m_TargetWorld = targetWorld;
        }

        private int count {
            get {
                return m_Count;
            }
        }

        private string botGroupName {
            get {
                return m_BotGroupName;
            }
        }

        private int counter {
            get {
                return m_Counter;
            }
        }

        private string targetWorld {
            get {
                return m_TargetWorld;
            }
        }

        private bool EventValidType(BaseEvent evt) {
            ContractEvent contractEvent = evt as ContractEvent;
            return (evt.eventType == EventType.GameObjectDeath) && 
                (contractEvent != null) && 
                (evt.source != null) && 
                (contractEvent.contractId == id);
        }

        private bool ContractValid() {
            return (state == ContractState.accepted) && CurrentWorldValid();
        }

        private bool CurrentWorldValid() {
            return (contractOwner.nebulaObject != null) && 
                ((contractOwner.nebulaObject.mmoWorld().Zone.Id == targetWorld) || string.IsNullOrEmpty(targetWorld));
        }

        private bool BotInValidGroup(BaseEvent evt) {
            var bot = evt.source.GetComponent<BotObject>();
            if(bot != null ) {
                if (bot.botGroup == botGroupName || (string.IsNullOrEmpty(botGroupName))) {
                    if(bot.HasDamager(contractOwner.nebulaObject.Id)) {
                        return true;
                    }
                }
            }
            return false;
        }

        private void IncrementCounter() { m_Counter++; }

        public override ContractCheckStatus CheckEvent(BaseEvent evt) {

            if(EventValidType(evt) && ContractValid() ) {

                var contractEvent = evt as ContractEvent;
                if(BotInValidGroup(evt)) {

                    IncrementCounter();
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
            return ContractCheckStatus.none;
        }
    }
}
