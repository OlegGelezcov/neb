using Common;
using ServerClientCommon;
using System.Collections;
using System.Collections.Generic;

namespace Nebula.Client
{
    public class ClientPlayerCharacter : IInfoParser
    {
        public string CharacterId { get; private set; }
        public string CharacterName { get; private set; }
        public int HomeWorkshop { get; private set; }
        public bool Deleted { get; private set; }
        public int Race { get; private set; }
        public Dictionary<ShipModelSlotType, string> Model { get; private set; }
        public string World { get; private set; }
        public int Exp { get; private set; }
        public string guildID { get; private set; }

        public ClientPlayerCharacter(Hashtable info )
        {
            this.ParseInfo(info);
        }

        public Hashtable ModelHash() {
            Hashtable result = new Hashtable();
            if(Model != null ) {
                foreach(var kModule in Model) {
                    result.Add((byte)kModule.Key, kModule.Value);
                }
            }
            return result;
        }


        public void ParseInfo(Hashtable info)
        {
            this.CharacterId = info.GetValue<string>((int)SPC.Id, string.Empty);
            this.CharacterName = info.GetValue<string>((int)SPC.Name, string.Empty);
            this.HomeWorkshop = info.GetValue<int>((int)SPC.Workshop, 0);
            this.Deleted = info.GetValue<bool>((int)SPC.Deleted, false);
            this.Race = info.GetValue<int>((int)SPC.Race, 0);
            World = info.GetValue<string>((int)SPC.WorldId, "");
            Exp = info.GetValue<int>((int)SPC.Exp, 0);

            if (info.ContainsKey((int)SPC.Guild) && info[(int)SPC.Guild] != null) {
                guildID = info.Value<string>((int)SPC.Guild, string.Empty);
            } else {
                guildID = string.Empty;
            }

            if (this.Model == null)
                this.Model = new Dictionary<ShipModelSlotType, string>();
            this.Model.Clear();

            Hashtable modelInfo = info.GetValue<Hashtable>((int)SPC.Model, new Hashtable());
            foreach(DictionaryEntry entry in modelInfo)
            {
                int slotType = (int)entry.Key;
                this.Model.Add((ShipModelSlotType)(byte)slotType, entry.Value.ToString());
            }
        }

        public override string ToString() {
            Hashtable modelHash = new Hashtable();
            foreach(var modelPair in Model) {
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
