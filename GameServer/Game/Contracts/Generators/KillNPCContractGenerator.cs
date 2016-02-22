using Common;
using Nebula.Contracts;
using Space.Game;
using System.Collections.Generic;

namespace Nebula.Game.Contracts.Generators {
    public class KillNPCContractGenerator : ContractGenerator {
        public override BaseContract Generate(Race race, int level, string sourceWorld, ContractManager manager, IContractResource resource) {
            var allowedByLevelAndCategoryContracts = resource.contracts.GetContracts(ContractCategory.killNPC, level);
            if (allowedByLevelAndCategoryContracts.Count == 0) {
                return null;
            }

            List<KillNPCContractData> contractsWithAllowedNPCs = new List<KillNPCContractData>();
            foreach(var c in allowedByLevelAndCategoryContracts) {
                var killNPCContract = c as KillNPCContractData;
                if(killNPCContract != null ) {
                    if(killNPCContract.HasNPCs(race, level)) {
                        contractsWithAllowedNPCs.Add(killNPCContract);
                    }
                }
            }

            if(contractsWithAllowedNPCs.Count == 0 ) {
                return null;
            }

            KillNPCContractData resultContractData = contractsWithAllowedNPCs.AnyElement();
            var npcData = resultContractData.GetRandomNPC(race, level);
            if(npcData == null ) {
                return null;
            }

            KillNPCContract contract = new KillNPCContract(
                resultContractData.id,
                0,
                sourceWorld,
                manager,
                npcData.name,
                npcData.RandomZone()
                );
            return contract;
        }
    }
}
