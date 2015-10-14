using Common;
using ServerClientCommon;
using System.Collections;

namespace Nebula.Client.Planets {
    public class PlanetSlot : IInfoParser {

        public byte slotNumber { get; private set; }
        public bool free { get; private set; }
        public string assignedStationID { get; private set; }
        public byte assignedStationType { get; private set; }
        public byte assignedStationRace { get; private set; }
        public string miningStationOwnedPlayerID { get; private set; }

        public void ParseInfo(Hashtable info) {
            slotNumber = info.GetValue<byte>((int)SPC.Index, 255);
            free = info.GetValue<bool>((int)SPC.Free, false);
            assignedStationID = info.GetValue<string>((int)SPC.AssignedStationID, string.Empty);
            assignedStationType = info.GetValue<byte>((int)SPC.AssignedStationType, 0);
            assignedStationRace = info.GetValue<byte>((int)SPC.AssignedStationRace, 0);
            miningStationOwnedPlayerID = info.GetValue<string>((int)SPC.MiningStationOwnedPlayer, string.Empty);
        }

        public PlanetSlot(Hashtable info) {
            ParseInfo(info);
        }
    }
}
