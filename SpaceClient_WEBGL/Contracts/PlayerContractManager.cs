using Common;
using ExitGames.Client.Photon;
using Nebula.Client.Utils;
using System.Collections.Generic;

namespace Nebula.Client.Contracts {
    public class PlayerContractManager : IInfoParser {
        private const int kCompletedContractsKey = 1;
        private const int kActiveContractsKey = 2;
        private readonly ContractFactory m_ContractFactory = new ContractFactory();

        public Dictionary<string, BaseContract> completedContracts { get; private set; }
        public Dictionary<string, BaseContract> activeContracts { get; private set; }

        public PlayerContractManager() {
            completedContracts = new Dictionary<string, BaseContract>();
            activeContracts = new Dictionary<string, BaseContract>();
        }

        public Hashtable Dump() {
            Hashtable completedHash = new Hashtable();
            foreach(var pcc in completedContracts ) {
                completedHash.Add(pcc.Key, pcc.Value.Dump());
            }

            Hashtable activeHash = new Hashtable();
            foreach(var pac in activeContracts ) {
                activeHash.Add(pac.Key, pac.Value.Dump());
            }
            return new Hashtable {
                {"completed", completedHash},
                {"active", activeHash }
            };
        }

        public void ParseInfo(Hashtable hash) {
            completedContracts.Clear();
            activeContracts.Clear();

            Hashtable completedHsh = hash.GetValueHash(kCompletedContractsKey);
            Hashtable activeHash = hash.GetValueHash(kActiveContractsKey);

            if(completedHsh != null ) {
                foreach(System.Collections.DictionaryEntry entry in completedHsh) {
                    string key = entry.Key.ToString();
                    Hashtable contractHash = entry.Value as Hashtable;
                    if(contractHash != null ) {
                        var contract = m_ContractFactory.Create(contractHash);
                        if(contract != null ) {
                            if(false == completedContracts.ContainsKey(key)) {
                                completedContracts.Add(key, contract);
                            }
                        }
                    }
                }
            }

            if(activeHash != null ) {
                foreach(System.Collections.DictionaryEntry entry in activeHash) {
                    string key = entry.Key.ToString();
                    Hashtable contractHash = entry.Value as Hashtable;
                    if(contractHash != null ) {
                        var contract = m_ContractFactory.Create(contractHash);
                        if(contract != null ) {
                            if(false == activeContracts.ContainsKey(key)) {
                                activeContracts.Add(key, contract);
                            }
                        }
                    }
                }
            }
        }

        public BaseContract ContractAccepted(Hashtable hash) {
            var contract = m_ContractFactory.Create(hash);
            if(contract != null ) {
                if(activeContracts.ContainsKey(contract.id)) {
                    activeContracts.Remove(contract.id);
                }
                activeContracts.Add(contract.id, contract);
            }
            return contract;
        }

        public BaseContract ContractCompleted(Hashtable hash) {
            var contract = m_ContractFactory.Create(hash);
            if(contract != null ) {
                if(activeContracts.ContainsKey(contract.id)) {
                    activeContracts.Remove(contract.id);
                }
                if(false == completedContracts.ContainsKey(contract.id)) {
                    completedContracts.Add(contract.id, contract);
                }
            }
            return contract;
        }

        public BaseContract ContractReady(Hashtable hash) {
            var contract = m_ContractFactory.Create(hash);
            if(contract != null ) {
                if(activeContracts.ContainsKey(contract.id)) {
                    activeContracts.Remove(contract.id);
                }
                activeContracts.Add(contract.id, contract);
            }
            return contract;
        }

        public BaseContract ContractStageChanged(Hashtable hash) {
            var contract = m_ContractFactory.Create(hash);
            if(contract != null ) {
                if(activeContracts.ContainsKey(contract.id)) {
                    activeContracts.Remove(contract.id);
                }
                activeContracts.Add(contract.id, contract);
            }
            return contract;
        }
    }
}
