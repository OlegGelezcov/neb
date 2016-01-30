using Common;
using ExitGames.Logging;
using ServerClientCommon;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Game.Contracts {
    public class ContractFactory {
        private static readonly ILogger s_Log = LogManager.GetCurrentClassLogger();

        public BaseContract Create(Hashtable hash) {
            var category = (ContractCategory)hash.GetValue<int>((int)SPC.ContractCategory, (int)ContractCategory.unknown);
            switch(category) {
                case ContractCategory.killNPC:
                    return new KillNPCContract(hash);
                default: {
                        s_Log.ErrorFormat("not exist contract catgeory: {0}", category);
                        return null;
                    }             
            }
        }
    }
}
