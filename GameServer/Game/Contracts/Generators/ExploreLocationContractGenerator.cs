using Common;
using Nebula.Contracts;
using Space.Game;
using System.Collections.Generic;

namespace Nebula.Game.Contracts.Generators {
    public class ExploreLocationContractGenerator : ContractGenerator {
        public override BaseContract Generate(Race race, int level, string sourceWorld, ContractManager manager, IContractResource resource) {
            var allowedByLevelAndCategoryContracts = resource.contracts.GetContracts(ContractCategory.exploreLocation, level);
            if (allowedByLevelAndCategoryContracts.Count == 0) {
                s_Log.InfoFormat("not found contracts for level and category");
                return null;
            }

            List<ExploreLocationContractData> contractsWithAllowedLocations = new List<ExploreLocationContractData>();
            foreach(var c in allowedByLevelAndCategoryContracts) {
                var exploreLocationContract = c as ExploreLocationContractData;
                if(exploreLocationContract != null ) {
                    if(exploreLocationContract.Has(race, level)) {
                        contractsWithAllowedLocations.Add(exploreLocationContract);
                    }
                }
            }

            if(contractsWithAllowedLocations.Count == 0 ) {
                s_Log.InfoFormat("not found contracts with locations for level and category");
                return null;
            }

            ExploreLocationContractData resultContractData = contractsWithAllowedLocations.AnyElement();
            var locationData = resultContractData.GetRandom(race, level);
            if(locationData == null ) {
                s_Log.InfoFormat("location data is null");
                return null;
            }

            ExploreLocationContract contract = new ExploreLocationContract(
                resultContractData.id,
                0,
                sourceWorld,
                manager,
                locationData.name,
                locationData.RandomZone()
                );
            s_Log.InfoFormat("contract generated succsessfully");

            return contract;
        }
    }
}
