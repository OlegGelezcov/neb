using Common;
using ExitGames.Logging;
using Nebula.Contracts;

namespace Nebula.Game.Contracts.Generators {
    public abstract class ContractGenerator {
        protected static readonly ILogger s_Log = LogManager.GetCurrentClassLogger();

        public abstract BaseContract Generate(Race race, int level, string sourceWorld, ContractManager manager, IContractResource resource);

        public static ContractGenerator Create(ContractCategory category) {
            switch(category) {
                case ContractCategory.killNPCGroup:
                    return new KillNPCGroupContractGenerator();
                case ContractCategory.killNPC:
                    return new KillNPCContractGenerator();
                case ContractCategory.exploreLocation:
                    return new ExploreLocationContractGenerator();
                case ContractCategory.itemDelivery:
                    return new ItemDeliveryContractGenerator();
                case ContractCategory.foundItem:
                    return new FoundItemContractGenerator();
                case ContractCategory.killPlayer:
                    return new KillPlayerContractGenerator();
                case ContractCategory.destroyConstruction:
                    return new DestroyConstructionContractGenerator();
                case ContractCategory.upgradeCompanion:
                    return new UpgradePetContractGenerator();
                case ContractCategory.worldCapture:
                    return new WorldCaptureContractGenerator();
                default:
                    s_Log.ErrorFormat("contract generator for category: {0} don't exists", category);
                    return null;
            }
        }
    }
}
