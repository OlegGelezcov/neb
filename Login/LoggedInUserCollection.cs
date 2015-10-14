using ExitGames.Logging;
using System;
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
        public void OnUserLoggedIn(string loginId, string accessToken, string gameRefId, string login) {
            lock(syncRoot) {
                LoggedInUser user;
                if(this.TryGetValue(loginId, out user)) {
                    this.Remove(loginId);
                }
                this.Add(loginId, new LoggedInUser { LoginId = loginId, AccessToken = accessToken, GameRefId = gameRefId, Login = login });
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
        }
    }
}
