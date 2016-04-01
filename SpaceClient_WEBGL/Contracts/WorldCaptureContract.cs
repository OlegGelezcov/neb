using Common;
using ExitGames.Client.Photon;
using Nebula.Client.Utils;
using ServerClientCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Client.Contracts {
    public class WorldCaptureContract : BaseContract {
        public Race race { get; private set; }

        public WorldCaptureContract(Hashtable hash)
            : base(hash) {
            race = (Race)(byte)hash.GetValueInt((int)SPC.Race);
        }

        public override bool TargetAtWorld(string worldId) {
            return false;
        }
        public override string GetTargetWorld() {
            return string.Empty;
        }
    }
}
