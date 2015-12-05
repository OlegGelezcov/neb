namespace Nebula.Client {
    using Common;
    using System.Collections.Generic;
    using System;
    using System.Collections;
    using ServerClientCommon;
    

    public class ClientPlayerCharactersContainer : IInfoParser {

        public string GameRefId { get; private set;}
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

        public void ParseInfo(Hashtable info) {

            GameRefId = info.Value<string>((int)SPC.GameRefId);
            SelectedCharacterId = info.Value<string>((int)SPC.SelectedCharacterId);

            if (Characters == null) {
                Characters = new List<ClientPlayerCharacter>();
            } else {
                Characters.Clear();
            }

            Hashtable characters = info.GetValue<Hashtable>((int)SPC.Characters, new Hashtable());
            if(characters == null ) {
                return;
            }

            foreach(DictionaryEntry characterEntry in characters ) {
                Hashtable characterInfo = characterEntry.Value as Hashtable;
                if(characterInfo == null ) {
                    continue;
                }
                Characters.Add(new ClientPlayerCharacter(characterInfo));
            }
        }

        public bool HasSelectedCharacter() {
            return (false == string.IsNullOrEmpty(SelectedCharacterId));
        }

        public ClientPlayerCharacter SelectedCharacter() {
            if(false == HasSelectedCharacter()) {
                return null;
            }
            foreach(var character in Characters ) {
                if(character.CharacterId == SelectedCharacterId) {
                    return character;
                }
            }
            return null;
        }

        public override string ToString() {
            Hashtable charactersHash = new Hashtable();
            foreach(var ch in Characters) {
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