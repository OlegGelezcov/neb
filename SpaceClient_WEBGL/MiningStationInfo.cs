using Common;
using GameMath;
using ServerClientCommon;
using ExitGames.Client.Photon;
using Nebula.Client.Utils;

namespace Nebula.Client {
    /// <summary>
    /// Hold info about mining station
    /// </summary>
    public class MiningStationInfo : IInfoParser {

        /// <summary>
        /// nebula element from this station
        /// </summary>
        public string nebulaElementID { get; private set; }

        /// <summary>
        /// current count of elements which station has
        /// </summary>
        public int currentCount { get; private set; }

        /// <summary>
        /// max count  of elements which station can hold
        /// </summary>
        public int maxCount { get; private set; }

        /// <summary>
        /// id of planet where station attached to
        /// </summary>
        public string planetID { get; private set; }
        /// <summary>
        /// Station owned player ID
        /// </summary>
        public string ownedPlayerID { get; private set; }
        /// <summary>
        /// Current count of items to warn out
        /// </summary>
        public int warnCount { get; private set; }
        /// <summary>
        /// Max count items when warn out occurs
        /// </summary>
        public int totalWarnCount { get; private set; }
        /// <summary>
        /// This station item ID
        /// </summary>
        public string itemID { get; private set; }
        /// <summary>
        /// This station item type (must be Bot)
        /// </summary>
        public byte itemType { get; private set; }

        public MiningStationInfo(Hashtable info) {
            ParseInfo(info);
        }

        public void ParseInfo(Hashtable info) {
            nebulaElementID = info.GetValueString((int)SPC.NebulaElementId);
            currentCount = info.GetValueInt((int)SPC.Count);
            maxCount = info.GetValueInt((int)SPC.MaxCount);
            planetID = info.GetValueString((int)SPC.Source);
            ownedPlayerID = info.GetValueString((int)SPC.MiningStationOwnedPlayer);
            warnCount = info.GetValueInt((int)SPC.WarnCount);
            totalWarnCount = info.GetValueInt((int)SPC.MaxWarnCount);
            itemID = info.GetValueString((int)SPC.ItemId);
            itemType = info.GetValueByte((int)SPC.ItemType);
        }

        public float warnPercent {
            get {
                if (totalWarnCount == 0) {
                    return 1f;
                }
                return Mathf.Clamp01((float)warnCount / (float)totalWarnCount);
            }
        }
    }
}
