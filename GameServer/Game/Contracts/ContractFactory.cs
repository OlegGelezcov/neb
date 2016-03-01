using Common;
using ExitGames.Logging;
using ServerClientCommon;
using System.Collections;

namespace Nebula.Game.Contracts {
    public class ContractFactory {
        private static readonly ILogger s_Log = LogManager.GetCurrentClassLogger();

        public BaseContract Create(Hashtable hash, ContractManager owner) {
            var category = (ContractCategory)hash.GetValue<int>((int)SPC.ContractCategory, (int)ContractCategory.unknown);
            switch(category) {
                case ContractCategory.killNPC:
                    return new KillNPCContract(hash, owner );
                case ContractCategory.killNPCGroup:
                    return new KillNPCGroupContract(hash, owner);
                case ContractCategory.exploreLocation:
                    return new ExploreLocationContract(hash, owner);
                case ContractCategory.itemDelivery:
                    return new ItemDeliveryContract(hash, owner);
                case ContractCategory.foundItem:
                    return new FoundItemContract(hash, owner);
                case ContractCategory.killPlayer:
                    return new KillPlayerContract(hash, owner);
                default: {
                        s_Log.ErrorFormat("not exist contract catgeory: {0}", category);
                        return null;
                    }             
            }
        }
    }
}
