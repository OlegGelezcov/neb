using Common;
using ExitGames.Client.Photon;
using Nebula.Client.Utils;
using ServerClientCommon;

namespace Nebula.Client {
    public class PlayerMarkedItem : IInfoParser {
        public string sourceId { get; private set; }
        public string prevId { get; private set; }
        public ItemType prevType { get; private set; }
        public string currentId { get; private set; }
        public ItemType currentType { get; private set; }

        public PlayerMarkedItem() {
            sourceId = string.Empty;
            prevId = string.Empty;
            prevType = ItemType.None;
            currentId = string.Empty;
            currentType = ItemType.None;
        }

        public PlayerMarkedItem(Hashtable hash) {
            ParseInfo(hash);
        }

        public void ParseInfo(Hashtable info) {
            sourceId = info.GetValueString((int)SPC.Source);
            prevId = info.GetValueString((int)SPC.PrevId);
            prevType = (ItemType)info.GetValueByte((int)SPC.PrevType);
            currentId = info.GetValueString((int)SPC.Id);
            currentType = (ItemType)info.GetValueByte((int)SPC.Type);
        }
    }
}
