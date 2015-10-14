using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using ServerClientCommon;

namespace Nebula.Client.Planets {

   
    public class PlanetObjectInfo : IInfoParser {
        public string nebulaElementID { get; private set; }
        public int maxSlots { get; private set; }
        public string id { get; private set; }
        public byte type { get; private set; }

        public Dictionary<byte, PlanetSlot> slots { get; private set; }

        public void ParseInfo(Hashtable info) {

            nebulaElementID = info.GetValue<string>((int)SPC.Template, string.Empty);
            maxSlots = info.GetValue<int>((int)SPC.MaxSlots, 0);
            id = info.GetValue<string>((int)SPC.Id, string.Empty);
            type = info.GetValue<byte>((int)SPC.Type, 0);
            slots = new Dictionary<byte, PlanetSlot>();
            Hashtable slotHash = info.GetValue<Hashtable>((int)SPC.PlanetSlots, new Hashtable());
            if(slotHash != null ) {
                foreach(DictionaryEntry entry in slotHash ) {
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
                if(slots != null ) {
                    return slots.Count((pk) => pk.Value.free == false);
                }
                return 0;
            }
        }

        public int count {
            get {
                if(slots != null ) {
                    return slots.Count;
                }
                return 0;
            }
        }
    }
}
