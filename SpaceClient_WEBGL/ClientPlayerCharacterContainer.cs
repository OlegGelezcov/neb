namespace Nebula.Client {
    using Common;
    using System.Collections.Generic;
    using System;
    using ExitGames.Client.Photon;
    using ServerClientCommon;
    using global::Common;

    public class ClientPlayerCharactersContainer : IInfoParser {

        public string GameRefId { get; private set; }
        public string SelectedCharacterId { get; private set; }

        public List<ClientPlayerCharacter> Characters { get; private set; }

        public ClientPlayerCharactersContainer() {
            GameRefId = "";
            SelectedCharacterId = "";
            Characters = new List<ClientPlayerCharacter>();
        }

        public ClientPlayerCharactersContainer(Hashtable info) {
            ParseInfo(info);
        }

        public ClientPlayerCharactersContainer(string json ) {
            Dictionary<string, object> charDict = MiniJSON.Json.Deserialize(json) as Dictionary<string, object>;
            if(charDict != null ) {
                if(charDict.ContainsKey("GameRefId")) {
                    object objGameRef = charDict["GameRefId"];
                    if(objGameRef != null ) {
                        GameRefId = objGameRef.ToString();
                    } else {
                        GameRefId = string.Empty;
                    }
                } else {
                    GameRefId = string.Empty;
                }

                if(charDict.ContainsKey("SelectedCharacterId")) {
                    object objSelCharacter = charDict["SelectedCharacterId"];
                    if(objSelCharacter != null ) {
                        SelectedCharacterId = objSelCharacter.ToString();
                    } else {
                        SelectedCharacterId = string.Empty;
                    }
                } else {
                    SelectedCharacterId = string.Empty;
                }

                Characters = new List<ClientPlayerCharacter>();
                if(charDict.ContainsKey("Characters")) {
                    List<object> objCharacterList = charDict["Characters"] as List<object>;
                    if(objCharacterList != null ) {
                        foreach(var cObj in objCharacterList ) {
                            Dictionary<string, object> singleCharacterDict = cObj as Dictionary<string, object>;
                            if(singleCharacterDict != null ) {
                                Characters.Add(new ClientPlayerCharacter(singleCharacterDict));
                            }
                        }
                    }
                }
            }


        }

        public void ParseInfo(Hashtable info) {

            GameRefId = info.Value<string>((int)SPC.GameRefId);
            SelectedCharacterId = info.Value<string>((int)SPC.SelectedCharacterId);

            if (Characters == null) {
                Characters = new List<ClientPlayerCharacter>();
            } else {
                Characters.Clear();
            }

            Hashtable characters = info.GetValue<Hashtable>((int)SPC.Characters, new Hashtable());
            if (characters == null) {
                return;
            }

            foreach (System.Collections.DictionaryEntry characterEntry in characters) {
                Hashtable characterInfo = characterEntry.Value as Hashtable;
                if (characterInfo == null) {
                    continue;
                }
                Characters.Add(new ClientPlayerCharacter(characterInfo));
            }
        }

        public bool HasSelectedCharacter() {
            return (false == string.IsNullOrEmpty(SelectedCharacterId));
        }

        public ClientPlayerCharacter SelectedCharacter() {
            if (false == HasSelectedCharacter()) {
                return null;
            }
            foreach (var character in Characters) {
                if (character.CharacterId == SelectedCharacterId) {
                    return character;
                }
            }
            return null;
        }

        public override string ToString() {
            Hashtable charactersHash = new Hashtable();
            foreach (var ch in Characters) {
                charactersHash.Add(ch.CharacterId, ch.ToString());
            }

            Hashtable hash = new Hashtable {
                {SPC.GameRefId, GameRefId },
                {SPC.SelectedCharacterId, SelectedCharacterId  },
                {SPC.Characters, charactersHash }
            };

            System.Text.StringBuilder builder = new System.Text.StringBuilder();
            CommonUtils.ConstructHashString(hash, 1, ref builder);
            return builder.ToString();
        }
    }
}