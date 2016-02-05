using Common;
using Nebula.Engine;
using Nebula.Game.Events;
using ServerClientCommon;
using System.Collections;

namespace Nebula.Game.Contracts {
    public abstract class BaseContract : IInfo {

        private string m_Id;
        private ContractState m_State;
        private int m_Stage;
        private string m_SourceWorld;
        private ContractCategory m_Category;
        private ContractManager m_ContractOwner;

        private float m_DeclineStartTime;
        private float m_DeclineEndTime;


        public BaseContract(Hashtable hash, ContractManager contractOwner ) {
            m_ContractOwner = contractOwner;
            ParseInfo(hash);
        }

        public BaseContract(string id, 
            ContractState state, 
            int stage, 
            string sourceWorld, 
            ContractCategory category, 
            ContractManager contractOwner) {

            m_Id = id;
            m_State = state;
            m_Stage = stage;
            m_SourceWorld = sourceWorld;
            m_Category = category;
            m_ContractOwner = contractOwner;
        }



        public override string ToString() {
            return string.Format("id: {0}, state: {1}, stage: {2}, source zone: {3}, category: {4}",
                id, state, stage, sourceWorld, category);
        }


        public string id {
            get {
                return m_Id;
            }
        }

        public ContractState state {
            get {
                return m_State;
            }
        }

        public int stage {
            get {
                return m_Stage;
            }
        }

        public string sourceWorld {
            get {
                return m_SourceWorld;
            }
        }

        public ContractCategory category {
            get {
                return m_Category;
            }
        }

        protected ContractManager contractOwner {
            get {
                return m_ContractOwner;
            }
        }

        public virtual Hashtable GetInfo() {
            return new Hashtable {
                {(int)SPC.Id, id },
                {(int)SPC.ContractState, (int)state },
                {(int)SPC.ContractStage, (int)stage },
                {(int)SPC.SourceWorld , sourceWorld },
                {(int)SPC.ContractCategory, (int)category },
                {(int)SPC.DeclineStart, m_DeclineStartTime },
                {(int)SPC.DeclineEnd, m_DeclineEndTime }
            };
        }

        public virtual void ParseInfo(Hashtable info) {
            m_Id = info.GetValue<string>((int)SPC.Id, string.Empty);
            m_State = (ContractState)info.GetValue<int>((int)SPC.ContractState, (int)ContractState.unknown);
            m_Stage = info.GetValue<int>((int)SPC.ContractStage, -1);
            m_SourceWorld = info.GetValue<string>((int)SPC.SourceWorld, string.Empty);
            m_Category = (ContractCategory)info.GetValue<int>((int)SPC.ContractCategory, (int)ContractCategory.unknown);
        }

        public abstract ContractCheckStatus CheckEvent(BaseEvent evt);

        private bool SetState(ContractState newState) {
            if (m_State != newState) {
                m_State = newState;
                return true;
            }
            return false;
        }

        protected void SetStage(int newStage) {
            m_Stage = newStage;
        }

        public bool Accept() {
            if(state == ContractState.proposed ) {
                if(SetState(ContractState.accepted)) {
                    return true; 
                }
            }
            return false;
        }

        public bool Decline() {
            if(state != ContractState.declined) {
                if(SetState(ContractState.declined)) {
                    m_DeclineStartTime = CommonUtils.SecondsFrom1970();
                    m_DeclineEndTime = m_DeclineStartTime + 2 * 60;
                    return true;
                }
            }
            return false;
        }

        public bool Complete() {
            if(state != ContractState.completed ) {
                if(SetState(ContractState.completed)) {
                    return true;
                }
            }
            return false;
        }

        protected bool Ready() {
            if(state  == ContractState.accepted ) {
                if(SetState(ContractState.ready)) {
                    return true;
                }
            }
            return false;
        }

        public ContractUpdateStatus Update(float time) {
            if(state == ContractState.declined ) {
                if(time >= m_DeclineEndTime) {
                    return ContractUpdateStatus.remove_to_trash;
                }
            }
            return ContractUpdateStatus.none;
        }

        public void Propose() {
            SetState(ContractState.proposed);
        }
    }

    public enum ContractCheckStatus {
        none = 1,
        stage_changed = 2,
        ready = 3
    }

    public enum ContractUpdateStatus {
        none = 1,
        remove_to_trash = 2
    }
}
