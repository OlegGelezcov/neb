using ServerClientCommon;
using System;
using System.Collections.Generic;
using System.Collections;

namespace TestClient {
    public static class Events {
        public static event Action<List<ServerInfo>> ServersReceived;
        public static event Action<string> GameRefIdReceived;
        public static event Action<Hashtable> PlayerCharactersReceived;
        public static event Action<string> CharacterCreated;
        public static event Action<string> CharacterSelected;


        public static void EvtServersReceived(List<ServerInfo> servers) {
            if(ServersReceived != null) {
                ServersReceived(servers);
            }
        }

        public static void EvtGameRefIdReceived(string gameRefId ) {
            if(GameRefIdReceived != null) {
                GameRefIdReceived(gameRefId);
            }
        }

        public static void EvtPlayerCharactersReceived(Hashtable playerCharacters) {
            if(PlayerCharactersReceived != null ) {
                PlayerCharactersReceived(playerCharacters);
            }
        }

        public static void EvtCharacterCreated(string characterId ) {
            if(CharacterCreated != null ) {
                CharacterCreated(characterId);
            }
        }

        public static void EvtCharacterSelected(string characterID ) {
            if(CharacterSelected != null) {
                CharacterSelected(characterID);
            }
        }
    }
}
