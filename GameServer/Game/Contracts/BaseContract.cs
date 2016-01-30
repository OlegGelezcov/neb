using Common;
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

        public BaseContract(Hashtable hash) {
            ParseInfo(hash);
        }

        public BaseContract(string id, ContractState state, int stage, string sourceWorld, ContractCategory category) {
            m_Id = id;
            m_State = state;
            m_Stage = stage;
            m_SourceWorld = sourceWorld;
            m_Category = category;
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

        public virtual Hashtable GetInfo() {
            return new Hashtable {
                {(int)SPC.Id, id },
                {(int)SPC.ContractState, (int)state },
                {(int)SPC.ContractStage, (int)stage },
                {(int)SPC.SourceWorld , sourceWorld },
                {(int)SPC.ContractCategory, (int)category }
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

        protected bool SetState(ContractState newState) {
            if (m_State != newState) {
                m_State = newState;
                return true;
            }
            return false;
        }

        protected void SetStage(int newStage) {
            m_Stage = newStage;
        }

        public void Complete() {
            SetState(ContractState.completed);
        }
    }

    public enum ContractCheckStatus {
        none = 1,
        stage_changed = 2,
        ready = 3
    }
}
