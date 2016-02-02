using Common;
using ExitGames.Client.Photon;
using Nebula.Client.Utils;
using ServerClientCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Client.Contracts {
    public class ContractFactory {
        public BaseContract Create(Hashtable hash ) {
            var category = (ContractCategory)hash.GetValueInt((int)SPC.ContractCategory);
            switch(category) {
                case ContractCategory.killNPC:
                    return new KillNPCContract(hash);
                case ContractCategory.killNPCGroup:
                    return new KillNPCGroupContract(hash);
                default:
                    return null;
            }
        }
    }
}
