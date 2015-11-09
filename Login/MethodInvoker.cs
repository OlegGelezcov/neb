// MethodInvoker.cs
// Nebula
//
// Created by Oleg Zheleztsov on Thursday, November 5, 2015 4:21:24 PM
// Copyright (c) 2015 KomarGames. All rights reserved.
//
using ExitGames.Logging;
using ServerClientCommon;

namespace Login {
    public class MethodInvoker {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

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

        /// <summary>
        /// Force expire time on user ( for testing )
        /// </summary>
        /// <param name="login">Login of user</param>
        /// <param name="gameRef">GameRef of user</param>
        /// <returns>Return status of operation</returns>
        public bool ExpireTime(string login, string gameRef) {
            var database = application.DbUserLogins;
            LoginReturnCode code = LoginReturnCode.Ok;
            var dbUser = database.GetExistingUserForGameRef(login, gameRef);
            
            if(dbUser == null ) {
                log.InfoFormat("ExpireTime: db user not found, code = {0} [red]", code);
                return false;
            }
            log.InfoFormat("ExpireTime: dbuser force expire");
            dbUser.ForceExpire();
            database.SaveUser(dbUser);
            application.LogedInUsers.SendPassesUpdateEvent(dbUser);
            return true;
        }
    }
}
