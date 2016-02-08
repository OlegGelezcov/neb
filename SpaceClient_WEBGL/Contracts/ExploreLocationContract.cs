using ExitGames.Client.Photon;
using Nebula.Client.Utils;
using ServerClientCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Client.Contracts {
    public class ExploreLocationContract : BaseContract {
        public string locationName { get; private set; }
        public string targetWorld { get; private set; }

        public ExploreLocationContract(Hashtable hash)
            : base(hash) {
            locationName = hash.GetValueString((int)SPC.Group);
            targetWorld = hash.GetValueString((int)SPC.TargetWorld);
        }

        public override Hashtable Dump() {
            Hashtable hash =  base.Dump();
            hash.Add("location_name", locationName);
            hash.Add("target_world", targetWorld);
            return hash;
        }

        public override string ToString() {
            string baseString = base.ToString();
            string newString = string.Format("location name: {0}, target world: {1}", locationName, targetWorld);
            return baseString + System.Environment.NewLine + newString;
        }

        public override bool TargetAtWorld(string worldId) {
            return (false == string.IsNullOrEmpty(worldId)) &&
                (targetWorld == worldId);
        }
    }
}
