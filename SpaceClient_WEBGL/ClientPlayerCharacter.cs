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
        public int raceStatus { get; private set; }

        public ClientPlayerCharacter(Hashtable info) {
            this.ParseInfo(info);
        }


        public ClientPlayerCharacter(Dictionary<string, object> jsonDictionary) {
            if(jsonDictionary.ContainsKey("CharacterId")) {
                object objCharacterId = jsonDictionary["CharacterId"];
                if (objCharacterId != null) {
                    CharacterId = objCharacterId.ToString();
                } else {
                    CharacterId = string.Empty;
                }
            } else {
                CharacterId = string.Empty;
            }

            if(jsonDictionary.ContainsKey("Name")) {
                object objName = jsonDictionary["Name"];
                if(objName != null ) {
                    CharacterName = objName.ToString();
                } else {
                    CharacterName = string.Empty;
                }
            } else {
                CharacterName = string.Empty;
            }


            if(jsonDictionary.ContainsKey("Race")) {
                object objRace = jsonDictionary["Race"];
                if(objRace != null ) {
                    Race = int.Parse(objRace.ToString());
                } else {
                    Race = (int)Common.Race.Humans;
                }
            } else {
                Race = (int)Common.Race.Humans;
            }

            if(jsonDictionary.ContainsKey("Workshop")) {
                object objWorkshop = jsonDictionary["Workshop"];
                if(objWorkshop != null ) {
                    HomeWorkshop = int.Parse(objWorkshop.ToString());
                } else {
                    HomeWorkshop = (int)(byte)CommonUtils.RandomWorkshop((Race)(byte)Race);
                }
            } else {
                HomeWorkshop = (int)(byte)CommonUtils.RandomWorkshop((Race)(byte)Race);
            }

            if(jsonDictionary.ContainsKey("Deleted")) {
                object objDeleted = jsonDictionary["Deleted"];
                if(objDeleted != null ) {
                    Deleted = bool.Parse(objDeleted.ToString());
                } else {
                    Deleted = false;
                }
            } else {
                Deleted = false;
            }

            Model = new Dictionary<ShipModelSlotType, string>();
            if(jsonDictionary.ContainsKey("Model")) {
                List<object> modelObjList = jsonDictionary["Model"] as List<object>;
                if(modelObjList != null ) {
                    foreach(object objModule in modelObjList) {
                        if(objModule != null ) {
                            List<object> singleModuleObjList = objModule as List<object>;
                            if(singleModuleObjList != null ) {
                                if(singleModuleObjList.Count == 2 ) {
                                    ShipModelSlotType slotType = (ShipModelSlotType)(byte)int.Parse(singleModuleObjList[0].ToString());
                                    string moduleId = singleModuleObjList[1].ToString();
                                    Model.Add(slotType, moduleId);
                                }
                            }
                        }
                    }
                }
            }

            if(jsonDictionary.ContainsKey("WorldId")) {
                object objWorldId = jsonDictionary["WorldId"];
                if(objWorldId != null ) {
                    World = objWorldId.ToString();
                } else {
                    World = string.Empty;
                }
            } else {
                World = string.Empty;
            }

            if(jsonDictionary.ContainsKey("Exp")) {
                object objExp = jsonDictionary["Exp"];
                if(objExp != null ) {
                    Exp = int.Parse(objExp.ToString());
                } else {
                    Exp = 0;
                }
            } else {
                Exp = 0;
            }

            if(jsonDictionary.ContainsKey("guildID")) {
                object objGuild = jsonDictionary["guildID"];
                if(objGuild != null ) {
                    guildID = objGuild.ToString();
                } else {
                    guildID = string.Empty;
                }
            } else {
                guildID = string.Empty;
            }

            if(jsonDictionary.ContainsKey("raceStatus")) {
                object objRaceStatus = jsonDictionary["raceStatus"];
                if(objRaceStatus != null ) {
                    raceStatus = int.Parse(objRaceStatus.ToString());
                } else {
                    raceStatus = 0;
                }
            } else {
                raceStatus = 0;
            }

            if(jsonDictionary.ContainsKey("characterIcon")) {
                object objIcon = jsonDictionary["characterIcon"];
                if(objIcon != null ) {
                    icon = int.Parse(objIcon.ToString());
                } else {
                    icon = 0;
                }
            } else {
                icon = 0;
            }
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
            raceStatus = info.GetValueInt((int)SPC.RaceStatus);

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
                { SPC.Exp, Exp },
                { SPC.RaceStatus, raceStatus }
            };

            System.Text.StringBuilder builder = new System.Text.StringBuilder();
            CommonUtils.ConstructHashString(hash, 1, ref builder);
            return builder.ToString();
        }
    }
}
