using Common;
using Nebula.Game.Events;
using ServerClientCommon;
using System.Collections;
using System;
using Nebula.Game.Components;

namespace Nebula.Game.Contracts {
    public class KillNPCContract : BaseContract {
        private string m_BotName;
        private string m_TargetWorld;

        public override void ParseInfo(Hashtable info) {
            base.ParseInfo(info);
            m_BotName = info.GetValue<string>((int)SPC.Group, string.Empty);
            m_TargetWorld = info.GetValue<string>((int)SPC.TargetWorld, string.Empty);
        }

        public override string ToString() {
            string baseString = base.ToString();
            string addedString = string.Format("bot name: {0}, target world: {1}", m_BotName, m_TargetWorld);
            return baseString + System.Environment.NewLine + addedString;
        }

        public override Hashtable GetInfo() {
            Hashtable hash =  base.GetInfo();
            hash.Add((int)SPC.Group, m_BotName);
            hash.Add((int)SPC.TargetWorld, m_TargetWorld);
            return hash;
        }

        public KillNPCContract(Hashtable hash, ContractManager manager)
            : base(hash, manager) {
            m_BotName = hash.GetValue<string>((int)SPC.Group, string.Empty);
            m_TargetWorld = hash.GetValue<string>((int)SPC.TargetWorld, string.Empty);
        }

        public KillNPCContract(string id, ContractState state, int stage, string sourceWorld, ContractManager manager, string botName, string targetWorld)
            : base(id, state, stage, sourceWorld, ContractCategory.killNPC, manager) {
            m_BotName = botName;
            m_TargetWorld = targetWorld;
        }

        public string botName {
            get {
                return m_BotName;
            }
        }

        public string targetWorld {
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
                if(bot.botGroup == botName || (string.IsNullOrEmpty(botName))) {
                    if(bot.HasDamager(contractOwner.nebulaObject.Id)) {
                        return true;
                    }
                }
            }
            return false;
        }

        public override ContractCheckStatus CheckEvent(BaseEvent evt) {
            if(EventValidType(evt) && ContractValid()) {
                var contractEvent = evt as ContractEvent;
                if(BotInValidGroup(evt)) {
                    if(Ready()) {
                        return ContractCheckStatus.ready;
                    }
                }
            }
            return ContractCheckStatus.none;
        }
    }
}
