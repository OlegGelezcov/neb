using ExitGames.Client.Photon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Client.Contracts {
    public class UpgradePetContract : BaseContract {
        public UpgradePetContract(Hashtable hash)
            : base(hash) { }

        public override bool TargetAtWorld(string worldId) {
            return false;
        }

        public override string GetTargetWorld() {
            return string.Empty;
        }
    }
}
