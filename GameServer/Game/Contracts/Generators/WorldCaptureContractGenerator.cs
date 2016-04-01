using Common;
using Nebula.Contracts;
using Nebula.Game.Components;
using Space.Game;

namespace Nebula.Game.Contracts.Generators {
    public class WorldCaptureContractGenerator : ContractGenerator {

        public override BaseContract Generate(Race race, int level, string sourceWorld, ContractManager manager, IContractResource resource) {
            var contracts = resource.contracts.GetContracts(ContractCategory.worldCapture, level);
            if(contracts.Count == 0 ) {
                return null;
            }

            WorldCaptureContractData data = contracts.AnyElement() as WorldCaptureContractData;
            if(data == null ) {
                return null;
            }

            var playerRaceable = manager.GetComponent<RaceableObject>();
            if(playerRaceable == null ) {
                return null;
            }

            Race playerRace = (Race)playerRaceable.race;

            Race targetRace = data.GenerateTargetRace(playerRace);

            WorldCaptureContract result = new WorldCaptureContract(data.id, 0, sourceWorld, manager, targetRace);
            return result;
        }

    }
}
