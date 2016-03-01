using Common;
using Nebula.Contracts;
using Space.Game;

namespace Nebula.Game.Contracts.Generators {
    /// <summary>
    /// Generator for kill player contract
    /// </summary>
    public class KillPlayerContractGenerator : ContractGenerator {
        public override BaseContract Generate(Race race, int level, string sourceWorld, ContractManager manager, IContractResource resource) {
            var contracts = resource.contracts.GetContracts(ContractCategory.killPlayer, level);
            if(contracts.Count == 0 ) {
                return null;
            }

            KillPlayerContractData data = contracts.AnyElement() as KillPlayerContractData;
            if(data == null ) {
                return null;
            }

            KillPlayerContract result = new KillPlayerContract(data.id, 0, sourceWorld, manager, data.playerCount);
            return result;
        }
    }
}
