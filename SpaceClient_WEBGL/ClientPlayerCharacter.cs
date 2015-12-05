using Common;
using ServerClientCommon;
using ExitGames.Client.Photon;
using System.Collections.Generic;
using Nebula.Client.Utils;

namespace Nebula.Client {
    public class ClientPlayerCharacter : IInfoParser {
        public string CharacterId { get; private set; }
        public string CharacterName { get; private set; }
        public int HomeWorkshop { get; private set; }
        public bool Deleted { get; private set; }
        public int Race { get; private set; }
        public Dictionary<ShipModelSlotType, string> Model { get; private set; }
        public string World { get; private set; }
        public int Exp { get; private set; }
        public string guildID { get; private set; }

        public int icon { get; private set; }

        public ClientPlayerCharacter(Hashtable info) {
            this.ParseInfo(info);
        }

        public Hashtable ModelHash() {
            Hashtable result = new Hashtable();
            if (Model != null) {
                foreach (var kModule in Model) {
                    result.Add((byte)kModule.Key, kModule.Value);
                }
            }
            return result;
        }


        public void ParseInfo(Hashtable info) {
            this.CharacterId = info.GetValueString((int)SPC.Id);
            this.CharacterName = info.GetValueString((int)SPC.Name);
            this.HomeWorkshop = info.GetValueInt((int)SPC.Workshop);
            this.Deleted = info.GetValueBool((int)SPC.Deleted);
            this.Race = info.GetValueInt((int)SPC.Race);
            World = info.GetValueString((int)SPC.WorldId);
            Exp = info.GetValueInt((int)SPC.Exp);
            icon = info.GetValueInt((int)SPC.Icon);

            if (info.ContainsKey((int)SPC.Guild) && info[(int)SPC.Guild] != null) {
                guildID = info.GetValueString((int)SPC.Guild);
            } else {
                guildID = string.Empty;
            }

            if (this.Model == null)
                this.Model = new Dictionary<ShipModelSlotType, string>();
            this.Model.Clear();

            Hashtable modelInfo = info.GetValueHash((int)SPC.Model);
            foreach (System.Collections.DictionaryEntry entry in modelInfo) {
                int slotType = (int)entry.Key;
                this.Model.Add((ShipModelSlotType)(byte)slotType, entry.Value.ToString());
            }
        }

        public override string ToString() {
            Hashtable modelHash = new Hashtable();
            foreach (var modelPair in Model) {
                modelHash.Add(modelPair.Key, modelPair.Value);
            }

            Hashtable hash = new Hashtable {
                {SPC.Id, CharacterId  },
                {SPC.Name, CharacterName  },
                {SPC.Workshop, (Workshop)HomeWorkshop },
                {SPC.Deleted, Deleted  },
                {SPC.Race, (Race)Race },
                { SPC.Model, modelHash},
                { SPC.WorldId, World},
                { SPC.Exp, Exp }
            };

            System.Text.StringBuilder builder = new System.Text.StringBuilder();
            CommonUtils.ConstructHashString(hash, 1, ref builder);
            return builder.ToString();
        }
    }
}
