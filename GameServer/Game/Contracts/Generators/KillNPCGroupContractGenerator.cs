using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using Nebula.Contracts;
using Space.Game;

namespace Nebula.Game.Contracts.Generators {

    public class KillNPCGroupContractGenerator : ContractGenerator {

        public override BaseContract Generate(Race race, int level, string sourceWorld, ContractManager manager, IContractResource resource) {
            var allowedByLevelAndCategoryContracts = resource.contracts.GetContracts(ContractCategory.killNPCGroup, level);
            if(allowedByLevelAndCategoryContracts.Count == 0 ) {
                return null;
            }

            List<KillNPCGroupContractData> contractsWithAllowedGroups = new List<KillNPCGroupContractData>();
            foreach(var c in allowedByLevelAndCategoryContracts) {
                var killGroupContract = c as KillNPCGroupContractData;
                if(killGroupContract != null ) {
                    if(killGroupContract.HasGroups(race, level)) {
                        contractsWithAllowedGroups.Add(killGroupContract);
                    }
                }
            }

            if(contractsWithAllowedGroups.Count == 0 ) {
                return null;
            }

            KillNPCGroupContractData resultContractData = contractsWithAllowedGroups.AnyElement();

            var groupData = resultContractData.GetRandomGroup(race, level);
            if(groupData == null ) {
                return null;
            }

            KillNPCGroupContract contract = new KillNPCGroupContract(resultContractData.id, 
                 0, 
                sourceWorld, 
                manager, 
                groupData.count, 
                groupData.name, 
                groupData.RandomZone());
            return contract;
        }
    }
}
