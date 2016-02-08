using Common;
using Space.Game;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Nebula.Contracts {
    public class ContractDataCollection {
        private ConcurrentDictionary<string, ContractData> m_Contracts;

        public ContractDataCollection(XElement element) {

            m_Contracts = new ConcurrentDictionary<string, ContractData>();

            List<XElement> contractElements = element.Elements("contract").ToList();

            foreach(XElement e in contractElements) {
                var data = Create(e);
                if(data != null ) {
                    m_Contracts.TryAdd(data.id, data);
                }
            }
        }

        private ContractData Create(XElement element ) {
            ContractCategory category = (ContractCategory)Enum.Parse(typeof(ContractCategory), element.GetString("category"));
            switch(category) {
                case ContractCategory.killNPCGroup:
                    return new KillNPCGroupContractData(element);
                case ContractCategory.killNPC:
                    return new KillNPCContractData(element);
                case ContractCategory.exploreLocation:
                    return new ExploreLocationContractData(element);
                default:
                    return null;
            }
        }

        public int count {
            get {
                return m_Contracts.Count;
            }
        }

        public List<ContractData> GetContracts(int minLevel) {
            List<ContractData> filtered = new List<ContractData>();
            foreach(var pcd in m_Contracts) {
                if(minLevel >= pcd.Value.minLevel ) {
                    filtered.Add(pcd.Value);
                }
            }
            return filtered;
        }

        public int GetContractCount(int level) {
            return GetContracts(level).Count;
        }



        public ContractData GetContract(string id) {
            ContractData data;
            if(m_Contracts.TryGetValue(id, out data)) {
                return data;
            }
            return null;
        }

        private List<ContractData> GetContracts(ContractCategory category) {
            List<ContractData> categoryContracts = new List<ContractData>();
            foreach(var pc in m_Contracts) {
                if(pc.Value.category == category) {
                    categoryContracts.Add(pc.Value);
                }
            }
            return categoryContracts;
        }

        public List<ContractData> GetContracts(ContractCategory category, int level ) {
            List<ContractData> categoryContracts = GetContracts(category);
            List<ContractData> leveledContracts = new List<ContractData>();
            foreach(var c in categoryContracts) {
                if(level >= c.minLevel ) {
                    leveledContracts.Add(c);
                }
            }
            return leveledContracts;
        }

        public ContractData GetRandom(ContractCategory category) {
            List<ContractData> dataList = new List<ContractData>();
            foreach(var pc in m_Contracts) {
                if(pc.Value.category == category) {
                    dataList.Add(pc.Value);
                }
            }
            if(dataList.Count > 0 ) {
                return dataList.AnyElement();
            }
            return null;
        }
    }
}
