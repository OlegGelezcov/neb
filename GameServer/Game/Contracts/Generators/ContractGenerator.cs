using Common;
using ExitGames.Logging;
using Nebula.Contracts;

namespace Nebula.Game.Contracts.Generators {
    public abstract class ContractGenerator {
        private static readonly ILogger s_Log = LogManager.GetCurrentClassLogger();

        public abstract BaseContract Generate(Race race, int level, string sourceWorld, ContractManager manager, IContractResource resource);

        public static ContractGenerator Create(ContractCategory category) {
            switch(category) {
                case ContractCategory.killNPCGroup:
                    return new KillNPCGroupContractGenerator();
                default:
                    s_Log.ErrorFormat("contract generator for category: {0} don't exists", category);
                    return null;
            }
        }
    }
}
