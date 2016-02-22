using Common;
using ExitGames.Logging;
using Nebula.Database;
using Nebula.Engine;
using Nebula.Game.Components;
using Nebula.Game.Events;
using Nebula.Game.Utils;
using Nebula.Inventory.DropList;
using Space.Game;
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
        private const int kProposedContractsKey = 3;

        private const float kUpdateInterval = 15;

        private static readonly ILogger s_Log = LogManager.GetCurrentClassLogger();

        private ConcurrentDictionary<string, BaseContract> m_CompletedContracts = new ConcurrentDictionary<string, BaseContract>();
        private ConcurrentDictionary<string, BaseContract> m_ActiveContracts = new ConcurrentDictionary<string, BaseContract>();

        private readonly ConcurrentDictionary<ContractCategory, BaseContract> m_ProposedContracts = new ConcurrentDictionary<ContractCategory, BaseContract>();

        private float m_UpdateTiemer = kUpdateInterval;
        private readonly List<string> m_TrashedContracts = new List<string>();

        private MmoMessageComponent m_MmoMessage;

        public void Load() {
            s_Log.InfoFormat("load contracts".Color(LogColor.dy));

            m_CompletedContracts.Clear();
            m_ActiveContracts.Clear();

            var factory = new ContractFactory();
            var character = GetComponent<PlayerCharacterObject>();
            var app = nebulaObject.mmoWorld().application;

            bool isNew = false;
            var contractsSave = ContractDatabase.instance(app).LoadContracts(character.characterId, resource, out isNew);
            if(contractsSave != null ) {

                if(contractsSave.completedContracts != null ) {
                    foreach(var cc in contractsSave.completedContracts) {
                        var contract = factory.Create(cc, this);
                        if(contract != null ) {
                            if(false == AddOrReplaceCompletedContract(contract)) {
                                s_Log.InfoFormat("fail to add completed contract: {0}".Color(LogColor.dy), contract.id);
                            }
                        }
                    }
                }

                if(contractsSave.activeContracts != null ) {
                    foreach(var ac in contractsSave.activeContracts) {
                        var contract = factory.Create(ac, this);
                        if(contract != null ) {
                            if(false == AddOrReplaceActiveContract(contract)) {
                                s_Log.InfoFormat("fail to add active contract: {0}".Color(LogColor.dy), contract.id);
                            }
                        }
                    }
                }
            }
        }

        public bool HasActiveContract(ContractCategory category) {
            foreach(var pac in m_ActiveContracts) {
                if(pac.Value.category == category ) {
                    return true;
                }
            }
            return false;
        }

        public bool HasActiveContract(string contractId ) {
            return m_ActiveContracts.ContainsKey(contractId);
        }

        public bool HasProposedContract(string id) {
            foreach(var pac in m_ProposedContracts) {
                if(pac.Value.id == id ) {
                    return true;
                }
            }
            return false;
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

            m_UpdateTiemer -= deltaTime;
            if(m_UpdateTiemer <= 0.0f ) {
                m_UpdateTiemer = kUpdateInterval;
                if(m_TrashedContracts.Count > 0 ) {
                    m_TrashedContracts.Clear();
                }

                float time = CommonUtils.SecondsFrom1970();
                foreach(var pac in m_ActiveContracts) {
                    if(pac.Value.Update(time) == ContractUpdateStatus.remove_to_trash) {
                        m_TrashedContracts.Add(pac.Key);
                    }
                }

                if(m_TrashedContracts.Count > 0 ) {
                    foreach(string cid in m_TrashedContracts ) {
                        BaseContract removedContract;
                        if(m_ActiveContracts.TryRemove(cid, out removedContract )) {
                            if(AddOrReplaceCompletedContract(removedContract)) {
                                m_MmoMessage.ContractCompleted(removedContract);
                            }
                        }
                    }
                }

            }

            base.Update(deltaTime);
        }

        private bool AddOrReplaceCompletedContract(BaseContract contract ) {

            //first remove all completed contracts this category
            ConcurrentBag<string> categoryContracts = new ConcurrentBag<string>();
            foreach(var pcc in m_CompletedContracts) {
                if(pcc.Value.category == contract.category) {
                    categoryContracts.Add(pcc.Value.id);
                }
            }

            foreach(var cid in categoryContracts) {
                BaseContract oldContract;
                m_CompletedContracts.TryRemove(cid, out oldContract);
            }

            //then add completed contract  -we have only single completed contract of category in completed contracts
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
                            s_Log.InfoFormat("contract: {0} is ready", pac.Value.id);
                            m_MmoMessage.ContractReady(pac.Value);
                            return true;
                        }
                    case ContractCheckStatus.stage_changed: {
                            s_Log.InfoFormat("contract: {0} is stage changed", pac.Value.id);
                            m_MmoMessage.ContractStageChanged(pac.Value);
                            return true;
                        }
                }
            }
            return false;
        }

        //================Public API=================================
        public Hashtable GetInfo() {
            Hashtable completedHash = new Hashtable();
            foreach (var pcc in m_CompletedContracts) {
                completedHash.Add(pcc.Key, pcc.Value.GetInfo());
            }

            Hashtable activeHash = new Hashtable();
            foreach (var pac in m_ActiveContracts) {
                activeHash.Add(pac.Key, pac.Value.GetInfo());
            }

            Hashtable proposedHash = new Hashtable();
            foreach(var pc in m_ProposedContracts ) {
                proposedHash.Add((int)pc.Key, pc.Value.GetInfo());
            }

            return new Hashtable {
                {kCompletedContractsKey, completedHash },
                {kActiveContractsKey,  activeHash},
                {kProposedContractsKey, proposedHash }
            };
        }


        public bool ProposeContract(BaseContract contract) {
            bool removedOld = true;
            if(m_ProposedContracts.ContainsKey(contract.category)) {
                BaseContract removedContract;
                if(false == m_ProposedContracts.TryRemove(contract.category, out removedContract)) {
                    removedOld = false;
                }
            }

            if(removedOld) {
                contract.Propose();
                if(m_ProposedContracts.TryAdd(contract.category, contract)) {
                    m_MmoMessage.ContractsUpdate(GetInfo());
                    return true;
                }
            }
            return false;
        }

        public bool AcceptProposedContract(string id, out BaseContract foundedContract) {
            foundedContract = null;
            foreach(var pc in m_ProposedContracts) {
                if(pc.Value.id == id ) {
                    foundedContract = pc.Value;
                    break;
                }
            }

            if(foundedContract != null ) {
                if(false == HasActiveContract(foundedContract.category)) {
                    BaseContract rc;
                    if(m_ProposedContracts.TryRemove(foundedContract.category, out rc)) {
                        
                        bool acceptedStatus = foundedContract.Accept();
                        if(AddOrReplaceActiveContract(foundedContract)) {
                            m_MmoMessage.ContractsUpdate(GetInfo());
                            m_MmoMessage.ContractAccepted(foundedContract);
                            if (acceptedStatus) {
                                foundedContract.OnAccepted();
                            }
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public bool DeclineContract(string id, out BaseContract declinedContract) {

            BaseContract foundedContract = null;
            foreach (var pc in m_ProposedContracts) {
                if (pc.Value.id == id) {
                    foundedContract = pc.Value;
                    break;
                }
            }

            if(foundedContract != null ) {
                foundedContract.Decline();

                if(HasActiveContract(foundedContract.category)) {
                    MoveActiveContractToTrash(foundedContract.category);
                }
                BaseContract rc;
                if (m_ProposedContracts.TryRemove(foundedContract.category, out rc)) {
                    if (AddOrReplaceActiveContract(foundedContract)) {
                        m_MmoMessage.ContractDeclined(foundedContract);
                        m_MmoMessage.ContractsUpdate(GetInfo());
                        declinedContract = foundedContract;
                        return true;
                    }
                }

            } else {
                foreach(var pac in m_ActiveContracts) {
                    if(pac.Key == id ) {
                        pac.Value.Decline();
                        m_MmoMessage.ContractDeclined(pac.Value);
                        m_MmoMessage.ContractsUpdate(GetInfo());
                        declinedContract = pac.Value;
                        return true;
                    }
                }
            }
            declinedContract = null;
            return false;
        }

        private void MoveActiveContractToTrash(ContractCategory category) {
            List<string> ids = new List<string>();
            foreach(var pac in m_ActiveContracts) {
                if(pac.Value.category == category ) {
                    ids.Add(pac.Key);
                }
            }
            foreach(string id in ids ) {
                BaseContract old;
                if(m_ActiveContracts.TryRemove(id, out old)) {
                    AddOrReplaceCompletedContract(old);
                }
            }
        }
        
        //private bool AcceptContract(BaseContract contract ) {
        //    if(contract.state == ContractState.accepted) {
        //        bool success =  AddOrReplaceActiveContract(contract);
        //        if(success) {
        //            m_MmoMessage.ContractAccepted(contract);
        //        }
        //        return success;
        //    }
        //    return false;
        //}

        public bool CompleteContract(string contractId, out BaseContract completedContract) {
            var contract = GetActiveContract(contractId);
            if(contract != null ) {
                if (contract.state == ContractState.ready) {
                    BaseContract removedActiveContract;
                    if (m_ActiveContracts.TryRemove(contractId, out removedActiveContract)) {
                        contract.Complete();
                        if (AddOrReplaceCompletedContract(contract)) {
                            m_MmoMessage.ContractCompleted(contract);
                            completedContract = contract;
                            return true;
                        }
                    }
                }

            }
            completedContract = null;
            return false;
        }

        public BaseContract GetActiveContract(string contractId ) {
            BaseContract contract;
            if(m_ActiveContracts.TryGetValue(contractId, out contract)) {
                return contract;
            }
            return null;
        }

        public void OnEnterStation() {
            s_Log.InfoFormat("OnEnterStation() message received at ContractManager".Color(LogColor.yellow));
            EnterStationEvent evt = new EnterStationEvent(nebulaObject);
            OnEvent(evt);
        }

        public List<string> FilterFoundItemContractForItems(List<string> dropList) {
            List<string> outputList = new List<string>();
            foreach(var pac in m_ActiveContracts ) {
                if(pac.Value.state == ContractState.accepted && pac.Value.stage == 0 ) {
                    if(pac.Value.category == ContractCategory.foundItem ) {
                        FoundItemContract contract = pac.Value as FoundItemContract;
                        if(dropList.Contains(contract.itemId)) {
                            outputList.Add(contract.itemId);
                        }
                    }
                }
            }
            return outputList;
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
