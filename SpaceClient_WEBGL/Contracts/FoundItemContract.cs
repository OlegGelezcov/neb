using ExitGames.Client.Photon;
using Nebula.Client.Utils;
using ServerClientCommon;

namespace Nebula.Client.Contracts {
    public class FoundItemContract : BaseContract {
        public string itemId { get; private set; }
        public string targetWorld { get; private set; }
        public string groupId { get; private set; }

        public FoundItemContract(Hashtable hash)
            : base(hash) {
            itemId = hash.GetValueString((int)SPC.ItemId);
            targetWorld = hash.GetValueString((int)SPC.TargetWorld);
            groupId = hash.GetValueString((int)SPC.Group);
        }

        public override Hashtable Dump() {
            Hashtable hash = base.Dump();
            hash.Add("item_id", itemId);
            hash.Add("target_world", targetWorld);
            hash.Add("group_id", groupId);
            return hash;
        }

        public override string ToString() {
            string baseString =  base.ToString();
            string newString = string.Format("item_id: {0}, target_world: {1}, group_id: {2}", itemId, targetWorld, groupId);
            return baseString + System.Environment.NewLine + newString;
        }

        public override bool TargetAtWorld(string worldId) {
            return (false == string.IsNullOrEmpty(worldId)) && (worldId == GetTargetWorld());
        }

        public override string GetTargetWorld() {
            string checkWorld = string.Empty;
            if (stage == 0) {
                checkWorld = sourceWorld;
            } else {
                checkWorld = targetWorld;
            }
            return checkWorld;
        }
    }
}
