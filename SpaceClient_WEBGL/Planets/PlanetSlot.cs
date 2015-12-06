using Common;
using ServerClientCommon;
using ExitGames.Client.Photon;
using Nebula.Client.Utils;

namespace Nebula.Client.Planets {
    public class PlanetSlot : IInfoParser {

        public byte slotNumber { get; private set; }
        public bool free { get; private set; }
        public string assignedStationID { get; private set; }
        public byte assignedStationType { get; private set; }
        public byte assignedStationRace { get; private set; }
        public string miningStationOwnedPlayerID { get; private set; }

        public void ParseInfo(Hashtable info) {
            slotNumber = info.GetValueByte((int)SPC.Index, 255);
            free = info.GetValueBool((int)SPC.Free);
            assignedStationID = info.GetValueString((int)SPC.AssignedStationID);
            assignedStationType = info.GetValueByte((int)SPC.AssignedStationType);
            assignedStationRace = info.GetValueByte((int)SPC.AssignedStationRace);
            miningStationOwnedPlayerID = info.GetValueString((int)SPC.MiningStationOwnedPlayer);
        }

        public PlanetSlot(Hashtable info) {
            ParseInfo(info);
        }
    }
}
