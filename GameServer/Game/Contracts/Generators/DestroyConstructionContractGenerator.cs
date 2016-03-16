using Common;
using Nebula.Contracts;
using Nebula.Game.Components;
using Space.Game;

namespace Nebula.Game.Contracts.Generators {
    public class DestroyConstructionContractGenerator : ContractGenerator {
        public override BaseContract Generate(Race race, int level, string sourceWorld, ContractManager manager, IContractResource resource) {
            var contracts = resource.contracts.GetContracts(ContractCategory.destroyConstruction, level);
            if(contracts.Count == 0 ) {
                s_Log.InfoFormat("contracts of type destroyConstruction is zero");
                return null;
            }
            DestroyConstructionContractData data = contracts.AnyElement() as DestroyConstructionContractData;
            if(data == null ) {
                s_Log.InfoFormat("contract data is zero");
                return null;
            }

            var sourceRaceableComponent = manager.GetComponent<RaceableObject>();
            if(sourceRaceableComponent == null ) {
                s_Log.InfoFormat("not found raceable component on player");
                return null;
            }

            
            Race sourceRace = (Race)sourceRaceableComponent.race;
            BotItemSubType constructionType = data.GenerateConstructionType();
            Race constructionRace = data.GenerateTargetRace(sourceRace);

            DestroyConstructionContract result = new DestroyConstructionContract(data.id, 0, sourceWorld, manager, constructionType, constructionRace);
            s_Log.InfoFormat("successfully generated contract destroy = {0} of race = {1}", constructionType, constructionRace);
            return result;
        }
    }
}
