using Common;
using ExitGames.Client.Photon;
using Nebula.Client.Utils;
using ServerClientCommon;

namespace Nebula.Client.Contracts {
    public class ContractFactory {
        public BaseContract Create(Hashtable hash ) {
            var category = (ContractCategory)hash.GetValueInt((int)SPC.ContractCategory);
            switch(category) {
                case ContractCategory.killNPC:
                    return new KillNPCContract(hash);
                case ContractCategory.killNPCGroup:
                    return new KillNPCGroupContract(hash);
                case ContractCategory.exploreLocation:
                    return new ExploreLocationContract(hash);
                case ContractCategory.itemDelivery:
                    return new ItemDeliveryContract(hash);
                case ContractCategory.foundItem:
                    return new FoundItemContract(hash);
                default:
                    return null;
            }
        }
    }
}
