using ExitGames.Client.Photon;
using Nebula.Client.Utils;
using ServerClientCommon;

namespace Nebula.Client.Contracts {
    public class ItemDeliveryContract : BaseContract {
        public string itemId { get; private set; }
        public string targetWorld { get; private set; }

        public ItemDeliveryContract(Hashtable hash) 
            : base(hash) {
            itemId = hash.GetValueString((int)SPC.ItemId);
            targetWorld = hash.GetValueString((int)SPC.TargetWorld);
        }

        public override Hashtable Dump() {
            var hash = base.Dump();
            hash.Add("item_id", itemId);
            hash.Add("target_world", targetWorld);
            return hash;
        }

        public override string ToString() {
            string baseString = base.ToString();
            string newString = string.Format("iem_id: {0}, target_world: {1}",
                itemId, targetWorld);
            return baseString + System.Environment.NewLine + newString;
        }

        public override bool TargetAtWorld(string worldId) {
            return (false == string.IsNullOrEmpty(worldId)) && (targetWorld == worldId);
        }
        public override string GetTargetWorld() {
            return targetWorld;
        }
    }
}
