using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using Nebula.Contracts;
using Space.Game;

namespace Nebula.Game.Contracts.Generators {
    public class ItemDeliveryContractGenerator : ContractGenerator  {
        public override BaseContract Generate(Race race, int level, string sourceWorld, ContractManager manager, IContractResource resource) {
            var allowedByLevelAndCategoryContracts = resource.contracts.GetContracts(ContractCategory.itemDelivery, level);
            if(allowedByLevelAndCategoryContracts.Count == 0 ) {
                return null;
            }
            List<ItemDeliveryContractData> contractsWithAllowedItems = new List<ItemDeliveryContractData>();
            foreach(var c in allowedByLevelAndCategoryContracts) {
                var itemDeliveryContract = c as ItemDeliveryContractData;
                if(itemDeliveryContract.Has(race, level)) {
                    contractsWithAllowedItems.Add(itemDeliveryContract);
                }
            }
            if(contractsWithAllowedItems.Count == 0 ) {
                return null;
            }
            ItemDeliveryContractData resultContractData = contractsWithAllowedItems.AnyElement();
            var itemData = resultContractData.GetRandom(race, level);
            if(itemData == null ) {
                return null;
            }
            string currentWorld = manager.nebulaObject.mmoWorld().GetID();
            var zones = itemData.zones.Where(z => z != currentWorld).ToList();
            if(zones.Count == 0 ) {
                return null;
            }

            ItemDeliveryContract contract = new ItemDeliveryContract(resultContractData.id,
                0,
                sourceWorld,
                manager,
                itemData.name,
                zones.AnyElement());
            return contract;
        }
    }
}
