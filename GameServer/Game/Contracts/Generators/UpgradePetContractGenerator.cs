using Common;
using Nebula.Contracts;
using Space.Game;

namespace Nebula.Game.Contracts.Generators {
    public class UpgradePetContractGenerator : ContractGenerator {
        public override BaseContract Generate(Race race, int level, string sourceWorld, ContractManager manager, IContractResource resource) {
            var contracts = resource.contracts.GetContracts(ContractCategory.upgradeCompanion, level);
            if(contracts.Count == 0 ) {
                s_Log.InfoFormat("contracts of type upgradeCompanion is zero");
                return null;
            }
            UpgradePetContractData data = contracts.AnyElement() as UpgradePetContractData;
            if(data == null ) {
                return null;
            }

            UpgradePetContract result = new UpgradePetContract(data.id, 0, sourceWorld, manager);
            return result;
        }
    }
}
