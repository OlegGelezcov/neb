using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using Nebula.Contracts;
using Space.Game;
using Nebula.Game.Utils;

namespace Nebula.Game.Contracts.Generators {
    public class FoundItemContractGenerator : ContractGenerator  {
        public override BaseContract Generate(Race race, int level, string sourceWorld, ContractManager manager, IContractResource resource) {
            var levelTypeList = resource.contracts.GetContracts(ContractCategory.foundItem, level);
            if(levelTypeList.Count == 0 ) {
                s_Log.InfoFormat("itemTypeList is count 0".Color(LogColor.green));
                return null;
            }

            var raceLevelList = new List<FoundItemContractData>();
            foreach(var c in levelTypeList) {
                var contract = c as FoundItemContractData;
                if(contract != null ) {
                    if(contract.Has(race, level)) {
                        raceLevelList.Add(contract);
                    }
                }
            }

            if(raceLevelList.Count == 0 ) {
                s_Log.InfoFormat("raceLevelList count 0".Color(LogColor.green)); 
                return null;
            }

            var randomContract = raceLevelList.AnyElement();
            var randomItem = randomContract.GetRandom(race, level);
            if(randomItem == null) {
                s_Log.InfoFormat("random item is null".Color(LogColor.green));
                return null;
            }

            string npcZone = randomItem.RandomNpcZone();
            if(string.IsNullOrEmpty(npcZone)) {
                s_Log.InfoFormat("npcZone is empty".Color(LogColor.green));
                return null;
            }

            FoundItemContract retContract = new FoundItemContract(
                randomContract.id,
                0,
                npcZone,
                manager,
                randomItem.name,
                randomItem.name,
                randomItem.RandomZone()
                );
            return retContract;
        }
    }
}
