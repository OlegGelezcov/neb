using Common;
using GameMath;
using ServerClientCommon;
using System.Collections;

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
            nebulaElementID = info.GetValue<string>((int)SPC.NebulaElementId, string.Empty);
            currentCount = info.GetValue<int>((int)SPC.Count, 0);
            maxCount = info.GetValue<int>((int)SPC.MaxCount, 0);
            planetID = info.GetValue<string>((int)SPC.Source, string.Empty);
            ownedPlayerID = info.GetValue<string>((int)SPC.MiningStationOwnedPlayer, string.Empty);
            warnCount = info.GetValue<int>((int)SPC.WarnCount, 0);
            totalWarnCount = info.GetValue<int>((int)SPC.MaxWarnCount, 0);
            itemID = info.GetValue<string>((int)SPC.ItemId, string.Empty);
            itemType = info.GetValue<byte>((int)SPC.ItemType, (byte)0);
        }

        public float warnPercent {
            get {
                if(totalWarnCount == 0) {
                    return 1f;
                }
                return Mathf.Clamp01((float)warnCount / (float)totalWarnCount);
            }
        }
    }
}
