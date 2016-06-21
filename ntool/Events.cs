using Nebula.Client;
using ServerClientCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ntool {
    public static class Events {
        public static event System.Action<LoginReturnCode> e_LoginFailed;
        public static event System.Action<string, string> e_LoginSuccess;
        public static event System.Action<ClientPlayerCharactersContainer> e_CharactersReceived;
        public static event System.Action e_CharacterReceiveFail;
        public static event System.Action e_AllPeersConnected;
        public static event System.Action<int> e_UsersOnlineReceived;

        public static void EventAllPeersConnected() {
            if(e_AllPeersConnected != null ) {
                e_AllPeersConnected();
            }
        }

        public static void EventCharactersReceived(ClientPlayerCharactersContainer container) {
            if(e_CharactersReceived != null ) {
                e_CharactersReceived(container);
            }
        }

        public static void EventCharacterReceiveFail() {
            if(e_CharacterReceiveFail != null ) {
                e_CharacterReceiveFail();
            }
        }

        public static void EventLoginFailed(LoginReturnCode code) {
            if(e_LoginFailed != null ) {
                e_LoginFailed(code);
            }
        }

        public static void EventLoginSuccess(string login, string gameRef ) {
            if(e_LoginSuccess != null ) {
                e_LoginSuccess(login, gameRef);
            }
        }
        public static void EventUsersOnlineReceived(int count) {
            if(e_UsersOnlineReceived != null ) {
                e_UsersOnlineReceived(count);
            }
        }
    }
}
