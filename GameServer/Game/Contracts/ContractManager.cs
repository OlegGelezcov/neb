using Common;
using ExitGames.Logging;
using Nebula.Database;
using Nebula.Engine;
using Nebula.Game.Components;
using Nebula.Game.Events;
using Nebula.Game.Utils;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Game.Contracts {
    public class ContractManager : NebulaBehaviour, IInfoSource {

        private const int kCompletedContractsKey = 1;
        private const int kActiveContractsKey = 2;

        private static readonly ILogger s_Log = LogManager.GetCurrentClassLogger();

        private ConcurrentDictionary<string, BaseContract> m_CompletedContracts = new ConcurrentDictionary<string, BaseContract>();
        private ConcurrentDictionary<string, BaseContract> m_ActiveContracts = new ConcurrentDictionary<string, BaseContract>();

        private MmoMessageComponent m_MmoMessage;

        public void Load() {
            s_Log.InfoFormat("load contracts".Color(LogColor.dy));

            m_CompletedContracts.Clear();
            m_ActiveContracts.Clear();

            var factory = new ContractFactory();
            var character = GetComponent<PlayerCharacterObject>();
            bool isNew = false;
            var contractsSave = ContractDatabase.instance.LoadContracts(character.characterId, resource, out isNew);
            if(contractsSave != null ) {

                if(contractsSave.completedContracts != null ) {
                    foreach(var cc in contractsSave.completedContracts) {
                        var contract = factory.Create(cc);
                        if(contract != null ) {
                            if(false == AddOrReplaceCompletedContract(contract)) {
                                s_Log.InfoFormat("fail to add completed contract: {0}".Color(LogColor.dy), contract.id);
                            }
                        }
                    }
                }

                if(contractsSave.activeContracts != null ) {
                    foreach(var ac in contractsSave.activeContracts) {
                        var contract = factory.Create(ac);
                        if(contract != null ) {
                            if(false == AddOrReplaceActiveContract(contract)) {
                                s_Log.InfoFormat("fail to add active contract: {0}".Color(LogColor.dy), contract.id);
                            }
                        }
                    }
                }
            }
        }

        public ContractSave GetSave() {
            List<Hashtable> completedContracts = new List<Hashtable>();
            foreach(var pcc in m_CompletedContracts) {
                if(pcc.Value != null ) {
                    completedContracts.Add(pcc.Value.GetInfo());
                }
            }

            List<Hashtable> activeContracts = new List<Hashtable>();
            foreach(var pac in m_ActiveContracts) {
                if(pac.Value != null ) {
                    activeContracts.Add(pac.Value.GetInfo());
                }
            }

            return new ContractSave(completedContracts, activeContracts);
        }

        public override int behaviourId {
            get {
                return (int)ComponentID.ContractManager;
            }
        }

        public override void Awake() {
            base.Awake();
        }

        public override void Start() {
            base.Start();
            m_MmoMessage = GetComponent<MmoMessageComponent>();
        }

        public override void Update(float deltaTime) {
            base.Update(deltaTime);
        }

        private bool AddOrReplaceCompletedContract(BaseContract contract ) {
            bool removedSuccessfully = true;
            if(m_CompletedContracts.ContainsKey(contract.id)) {
                BaseContract oldContract;
                removedSuccessfully = m_CompletedContracts.TryRemove(contract.id, out oldContract);
            }
            if(removedSuccessfully) {
                return m_CompletedContracts.TryAdd(contract.id, contract);
            }
            return false;
        }

        private bool AddOrReplaceActiveContract(BaseContract contract) {
            bool removedSuccessfully = true;
            if(m_ActiveContracts.ContainsKey(contract.id)) {
                BaseContract oldContract;
                removedSuccessfully = m_ActiveContracts.TryRemove(contract.id, out oldContract);
            }
            if(removedSuccessfully) {
                return m_ActiveContracts.TryAdd(contract.id, contract);
            }
            return false;
        }


        public bool OnEvent(BaseEvent evt) {
            foreach(var pac in m_ActiveContracts) {
                var status = pac.Value.CheckEvent(evt);
                switch(status) {
                    case ContractCheckStatus.ready: {
                            s_Log.InfoFormat("contract: {0} is ready");
                            m_MmoMessage.ContractReady(pac.Value);
                            return true;
                        }
                    case ContractCheckStatus.stage_changed: {
                            s_Log.InfoFormat("contract: {0} is stage changed");
                            m_MmoMessage.ContractStageChanged(pac.Value);
                            return true;
                        }
                }
            }
            return false;
        }

        public Hashtable GetInfo() {
            Hashtable completedHash = new Hashtable();
            foreach (var pcc in m_CompletedContracts) {
                completedHash.Add(pcc.Key, pcc.Value.GetInfo());
            }

            Hashtable activeHash = new Hashtable();
            foreach (var pac in m_ActiveContracts) {
                activeHash.Add(pac.Key, pac.Value.GetInfo());
            }

            return new Hashtable {
                {kCompletedContractsKey, completedHash },
                {kActiveContractsKey,  activeHash}
            };
        }

        //================Public API=================================
        public bool AcceptContract(BaseContract contract ) {
            if(contract.state == ContractState.accepted) {
                bool success =  AddOrReplaceActiveContract(contract);
                if(success) {
                    m_MmoMessage.ContractAccepted(contract);
                }
                return success;
            }
            return false;
        }

        public bool CompleteContract(string contractId) {
            var contract = GetActiveContract(contractId);
            if(contract != null ) {
                BaseContract removedActiveContract;
                if(m_ActiveContracts.TryRemove(contractId, out removedActiveContract)) {
                    contract.Complete();
                    if(AddOrReplaceCompletedContract(contract)) {
                        m_MmoMessage.ContractCompleted(contract);
                        return true;
                    }
                }
            }
            return false;
        }

        public BaseContract GetActiveContract(string contractId ) {
            BaseContract contract;
            if(m_ActiveContracts.TryGetValue(contractId, out contract)) {
                return contract;
            }
            return null;
        }
    }

    public class ContractSave {
        private List<Hashtable> m_CompletedContracts;
        private List<Hashtable> m_ActiveContracts;

        public ContractSave(List<Hashtable> completed, List<Hashtable> active) {
            m_CompletedContracts = completed;
            m_ActiveContracts = active;
        }

        public List<Hashtable> completedContracts {
            get {
                return m_CompletedContracts;
            }
        }

        public List<Hashtable> activeContracts {
            get {
                return m_ActiveContracts;
            }
        }
    }
}
