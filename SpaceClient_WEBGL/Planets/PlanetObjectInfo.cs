using Common;
using System.Collections.Generic;
using System.Linq;
using ServerClientCommon;
using ExitGames.Client.Photon;
using Nebula.Client.Utils;

namespace Nebula.Client.Planets {


    public class PlanetObjectInfo : IInfoParser {
        public string nebulaElementID { get; private set; }
        public int maxSlots { get; private set; }
        public string id { get; private set; }
        public byte type { get; private set; }

        public Dictionary<byte, PlanetSlot> slots { get; private set; }

        public void ParseInfo(Hashtable info) {

            nebulaElementID = info.GetValueString((int)SPC.Template);
            maxSlots = info.GetValueInt((int)SPC.MaxSlots);
            id = info.GetValueString((int)SPC.Id);
            type = info.GetValueByte((int)SPC.Type);
            slots = new Dictionary<byte, PlanetSlot>();
            Hashtable slotHash = info.GetValueHash((int)SPC.PlanetSlots);
            if (slotHash != null) {
                foreach (System.Collections.DictionaryEntry entry in slotHash) {
                    byte key = (byte)entry.Key;
                    Hashtable slotInfo = (Hashtable)entry.Value;
                    slots.Add(key, new PlanetSlot(slotInfo));
                }
            }
        }

        public PlanetObjectInfo(Hashtable planetInfo) {
            ParseInfo(planetInfo);
        }

        public int countNonFreeSlots {
            get {
                if (slots != null) {
                    return slots.Count((pk) => pk.Value.free == false);
                }
                return 0;
            }
        }

        public int count {
            get {
                if (slots != null) {
                    return slots.Count;
                }
                return 0;
            }
        }

        public bool HasStationOfPlayer(string playerID) {
            if (slots == null) {
                return false;
            }
            foreach (var pSlot in slots) {
                if (pSlot.Value.miningStationOwnedPlayerID == playerID) {
                    return true;
                }
            }
            return false;
        }
    }
}
