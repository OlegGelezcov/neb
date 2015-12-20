// LoggedInUserCollection.cs
// Nebula
//
// Created by Oleg Zheleztsov on Thursday, November 5, 2015 4:23:25 PM
// Copyright (c) 2015 KomarGames. All rights reserved.
//
using ExitGames.Logging;
using Nebula.Server.Login;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Login {
    public class LoggedInUserCollection : Dictionary<string, LoggedInUser> {

        private readonly object syncRoot = new object();
        private LoginApplication application;

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();


        public LoggedInUserCollection(LoginApplication application) {
            this.application = application;
        }

        /// <summary>
        /// Called after checking access token and reading game ref id from database
        /// </summary>
        /// <param name="loginId"></param>
        /// <param name="accessToken"></param>
        /// <param name="gameRefId"></param>
        public void OnUserLoggedIn(FullUserAuth auth, LoginClientPeer peer) {
            lock(syncRoot) {
                LoggedInUser user;
                if(this.TryGetValue(auth.login, out user)) {
                    this.Remove(auth.login);
                }
                this.Add(auth.login, new LoggedInUser(auth, peer) );
                log.InfoFormat("added user = {0} yellow", auth.login);
            }
        }

        /// <summary>
        /// called when user log out make
        /// </summary>
        /// <param name="loginId"></param>
        public void OnLogOut(LoginId login) {
            lock(this.syncRoot) {
                if (login != null) {
                    this.Remove(login.value);
                    log.InfoFormat("removed user = {0} yellow", login.value);
                }
            }

            DeleteNotValidUsers();
        }


        private void DeleteNotValidUsers() {
            lock(syncRoot) {
                ConcurrentBag<string> keys = new ConcurrentBag<string>();

                foreach(var pclient in this ) {
                    if(pclient.Value.peer == null || pclient.Value.peer.Disposed) {
                        keys.Add(pclient.Key);
                    }
                }

                foreach (var k in keys) {
                    log.InfoFormat("delete not valid user = {0} [red]", k);
                    Remove(k);
                }
            }
        }
    }
}
