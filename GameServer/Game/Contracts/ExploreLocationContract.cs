using Common;
using Nebula.Game.Events;
using ServerClientCommon;
using System.Collections;

namespace Nebula.Game.Contracts {
    public class ExploreLocationContract : BaseContract  {
        private string m_LocationName;
        private string m_TargetWorld;

        public override void ParseInfo(Hashtable info) {
            base.ParseInfo(info);
            m_LocationName = info.GetValue<string>((int)SPC.Group, string.Empty);
            m_TargetWorld = info.GetValue<string>((int)SPC.TargetWorld, string.Empty);
        }

        public override string ToString() {
            string baseString = base.ToString();
            string addedString = string.Format("location name: {0}, target world: {1}", m_LocationName, m_TargetWorld);
            return baseString + System.Environment.NewLine + addedString;
        }
        public override Hashtable GetInfo() {
            Hashtable hash = base.GetInfo();
            hash.Add((int)SPC.Group, m_LocationName);
            hash.Add((int)SPC.TargetWorld, m_TargetWorld);
            return hash;
        }

        public ExploreLocationContract(Hashtable hash, ContractManager manager) : base(hash, manager) {
            m_LocationName = hash.GetValue<string>((int)SPC.Group, string.Empty);
            m_TargetWorld = hash.GetValue<string>((int)SPC.TargetWorld, string.Empty);
        }
        public ExploreLocationContract(string id, ContractState state, int stage, string sourceWorld, ContractManager manager, string locationName, string targetWorld)
            : base(id, state, stage, sourceWorld, ContractCategory.exploreLocation, manager) {
            m_LocationName = locationName;
            m_TargetWorld = targetWorld;
        }

        public string locationName {
            get {
                return m_LocationName;
            }
        }

        public string targetWorld {
            get {
                return m_TargetWorld;
            }
        }

        private bool EventValidType(BaseEvent evt) {
            return (evt.eventType == EventType.TriggerStay) || (evt.eventType == EventType.TriggerExit);
        }

        private bool EventValid(BaseEvent evt) {
            TriggerEvent triggerEvent = evt as TriggerEvent;
            return (EventValidType(evt)) &&
                (triggerEvent != null) &&
                (triggerEvent.source != null) &&
                (triggerEvent.source.GetComponent<LocationTrigger>()) &&
                (triggerEvent.source.GetComponent<LocationTrigger>().triggerName == locationName);
        }

        private bool ContractValid() {
            return (state == ContractState.accepted) && CurrentWorldValid();
        }

        private bool CurrentWorldValid() {
            return (contractOwner.nebulaObject != null) &&
                ((contractOwner.nebulaObject.mmoWorld().Zone.Id == targetWorld) || string.IsNullOrEmpty(targetWorld));
        }

        private bool TriggerTimerOk(BaseEvent evt) {
            TriggerEvent triggerEvent = evt as TriggerEvent;
            return (triggerEvent.interval >= 60.0f);
        }

        private bool TriggerTargetIsMyPlayer(BaseEvent evt) {
            TriggerEvent triggerEvent = evt as TriggerEvent;
            if(triggerEvent.target != null ) {
                if(triggerEvent.target.Id == contractOwner.nebulaObject.Id ) {
                    return true;
                }
            }
            return false;
        }

        public override ContractCheckStatus CheckEvent(BaseEvent evt) {
            if(EventValid(evt) && ContractValid()) {
                if (TriggerTargetIsMyPlayer(evt)) {
                    if (TriggerTimerOk(evt)) {
                        if (Ready()) {
                            return ContractCheckStatus.ready;
                        }
                    } else {
                        int interval = (int)(evt as TriggerEvent).interval;
                        //if (interval > stage) {
                        SetStage(interval);
                        return ContractCheckStatus.stage_changed;
                        //}
                    }
                }
            }
            return ContractCheckStatus.none;
        }
    }
}
