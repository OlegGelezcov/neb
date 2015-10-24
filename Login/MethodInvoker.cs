using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Login {
    public class MethodInvoker {

        public LoginApplication application { get; private set; }
        public LoginClientPeer peer { get; private set; }

        public MethodInvoker(LoginApplication app, LoginClientPeer impeer) {
            application = app;
            peer = impeer;
        }


        public bool MovePassToInventory(string login, string gameRef, string character, string targetServer) {
            return application.passManager.MovePassToInventory(login, gameRef, character, targetServer);
        }

        public bool MoveInventoryToPass(string login, string gameRef, string character, string item, string targetServer) {
            return application.passManager.MoveInventoryItemToPass(login, gameRef, character, item, targetServer);
        }

        public bool RequestPassUpdate(string login, string gameRef ) {
            var dbUser = application.DbUserLogins.GetExistingUserForGameRef(login, gameRef);
            if(dbUser != null ) {
                application.LogedInUsers.SendPassesUpdateEvent(dbUser);
                return true;
            }
            return false;
        }
    }
}
