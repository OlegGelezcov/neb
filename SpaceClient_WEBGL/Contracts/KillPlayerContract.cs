using ExitGames.Client.Photon;
using Nebula.Client.Utils;
using ServerClientCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Client.Contracts {
    public class KillPlayerContract : BaseContract {
        public int count { get; private set; }
        public int counter { get; private set; }

        public KillPlayerContract(Hashtable hash) 
            : base(hash) {
            count = hash.GetValueInt((int)SPC.Count);
            counter = hash.GetValueInt((int)SPC.Counter);
        }

        public override string ToString() {
            string baseStr = base.ToString();
            string newStr = string.Format("count: {0}, counter: {1}", count, counter);
            return baseStr +
                System.Environment.NewLine +
                newStr;
        }

        public override Hashtable Dump() {
            Hashtable hash = base.Dump();
            hash.Add("count", count);
            hash.Add("counter", counter);
            return hash;
        }
        public override bool TargetAtWorld(string worldId) {
            return false;
        }
        public override string GetTargetWorld() {
            return string.Empty;
        }
    }
}
