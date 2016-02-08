using ExitGames.Client.Photon;
using Nebula.Client.Utils;
using ServerClientCommon;

namespace Nebula.Client.Contracts {
    public class KillNPCContract : BaseContract {

        public string npcName { get; private set; }
        public string targetWorld { get; private set; }


        public KillNPCContract(Hashtable hash)
            : base(hash) {
            npcName = hash.GetValueString((int)SPC.Group);
            targetWorld = hash.GetValueString((int)SPC.TargetWorld);
        }

        public override Hashtable Dump() {
            Hashtable hash = base.Dump();
            hash.Add("npcname", npcName);
            hash.Add("targetworld", targetWorld);
            return hash;
        }

        public override string ToString() {
            string baseString =  base.ToString();
            string newString = string.Format("npc name: {0}, target world: {1}", npcName, targetWorld);
            return baseString + System.Environment.NewLine + newString;
        }

        public override bool TargetAtWorld(string worldId) {
            return (false == string.IsNullOrEmpty(worldId)) && (targetWorld == worldId);
        }
    }
}
