using ExitGames.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using ServerClientCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Login {
    public class DbReader {
        public MongoClient DbClient { get; private set; }
        public MongoServer DbServer { get; private set; }
        public MongoDatabase Database { get; private set; }
        public MongoCollection<DbUserLogin> UserLogins { get; private set; }

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public void Setup(string connectionString, string databaseName, string userLoginsCollectionName) {
            this.DbClient = new MongoClient(connectionString);
            this.DbServer = this.DbClient.GetServer();
            this.Database = this.DbServer.GetDatabase(databaseName);
            this.UserLogins = this.Database.GetCollection<DbUserLogin>(userLoginsCollectionName);
        }

        public bool ExistsLogin(string loginId) {
            var query = Query<DbUserLogin>.EQ(u => u.LoginId, loginId);
            return this.UserLogins.Count(query) > 0;
        }

        public bool ExistGameRef(string gameRef) {
            var query = Query<DbUserLogin>.EQ(u => u.GameRefId, gameRef);
            return this.UserLogins.Count(query) > 0;
        }

        public DbUserLogin Get(string loginId, string gameRefID, out LoginReturnCode code) {
            code = LoginReturnCode.Ok;

            if (!Regex.IsMatch(loginId, "^[a-zA-Z0-9]*$")) {
                code = LoginReturnCode.InvaligLoginCharacter;
                return null;
            }

            if (loginId.Length < 4) {
                code = LoginReturnCode.InvaligLoginLength;
                return null;
            }


            if (ExistsLogin(loginId)) {

                
                var query = Query<DbUserLogin>.EQ(u => u.LoginId, loginId);
                var userLogin = this.UserLogins.FindOne(query);

                if (userLogin.GameRefId != gameRefID) {
                    log.InfoFormat("Login {0} exists but game ref don't same, return error", loginId);
                    code = LoginReturnCode.UserWithSameLoginAlreadyExists;
                    return null;
                } else {
                    log.InfoFormat("Login {0} exists and game ref same, all OK", loginId);
                    return userLogin;
                }
            } else {

                if (ExistGameRef(gameRefID)) {
                    
                    UserLogins.Remove(Query<DbUserLogin>.EQ(u => u.GameRefId, gameRefID));

                    string newGameRef = Guid.NewGuid().ToString();

                    DbUserLogin newUserLogin = new DbUserLogin {
                        CreationTime = DateTime.UtcNow,
                        GameRefId = newGameRef,
                        LoginId = loginId
                    };
                    this.UserLogins.Insert(newUserLogin);
                    log.InfoFormat("login don't exists, but game ref {0} exists, remove old game ref and set for this game ref new login {1}", gameRefID, loginId);
                    code = LoginReturnCode.UpdateGameRefOnClient;

                    return newUserLogin;

                } else {
                    log.InfoFormat("its new user login {0} and game ref {1} don't exists", loginId, gameRefID);
                    DbUserLogin newUserLogin = new DbUserLogin {
                        CreationTime = DateTime.UtcNow,
                        GameRefId = gameRefID,
                        LoginId = loginId,
                    };
                    this.UserLogins.Insert(newUserLogin);
                    return newUserLogin;
                }
                
            }
        }
    }

    public class DbUserLogin {
        public ObjectId Id { get; set; }
        public string LoginId { get; set; }
        public string GameRefId { get; set; }
        public DateTime CreationTime { get; set; }
    }
}
