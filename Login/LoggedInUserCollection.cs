using ExitGames.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        public void OnUserLoggedIn(string login, string gameRef, LoginClientPeer peer) {
            lock(syncRoot) {
                LoggedInUser user;
                if(this.TryGetValue(login, out user)) {
                    this.Remove(login);
                }
                this.Add(login, new LoggedInUser(login, gameRef, peer) );
                log.InfoFormat("added user = {0} yellow", login);
            }
        }

        /// <summary>
        /// called when user log out make
        /// </summary>
        /// <param name="loginId"></param>
        public void OnLogOut(string loginId ) {
            lock(this.syncRoot) {
                this.Remove(loginId);
                log.InfoFormat("removed user = {0} yellow", loginId);
            }

            DeleteNotValidUsers();
        }

        public void SendPassesUpdateEvent(DbUserLogin dbUser) {
            lock(syncRoot) {
                LoggedInUser user;
                if(TryGetValue(dbUser.login, out user)) {
                    user.SendPassesUpdateEvent(dbUser);
                }
            }
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
