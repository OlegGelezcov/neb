using ExitGames.Client.Photon;
using Nebula.Client.Utils;
using ServerClientCommon;

namespace Nebula.Client.Contracts {
    public class KillNPCGroupContract : BaseContract {

        public int count { get; private set; }
        public string groupName { get; private set; }
        public int counter { get; private set; }
        public string targetWorld { get; private set; }

        public KillNPCGroupContract(Hashtable hash)
            : base(hash) {
            count = hash.GetValueInt((int)SPC.Count);
            groupName = hash.GetValueString((int)SPC.Group);
            counter = hash.GetValueInt((int)SPC.Counter);
            targetWorld = hash.GetValueString((int)SPC.TargetWorld);
        }

        public override string ToString() {
            string baseString =  base.ToString();
            string newString = string.Format("count: {0}, group: {1}, counter: {2}, target zone: {3}",
                count, groupName, counter, targetWorld);
            return baseString + System.Environment.NewLine + newString;
        }

        public override Hashtable Dump() {
            Hashtable hash = base.Dump();
            hash.Add("count", count);
            hash.Add("group", groupName);
            hash.Add("counter", counter);
            hash.Add("targetworld", targetWorld);
            return hash;
        }
    }
}
