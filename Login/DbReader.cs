using Common;
using ExitGames.Logging;
using Login.OperationHandlers;
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

        public const int NEW_USER_GAME_INTERVAL_IN_SECONDS = 172800;
        public const int TIME_FOR_PASS = 2592000;


        public MongoClient DbClient { get; private set; }
        public MongoServer DbServer { get; private set; }
        public MongoDatabase Database { get; private set; }
        public MongoCollection<DbUserLogin> UserLogins { get; private set; }

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private readonly LoginTextUtilities mLoginUtilities = new LoginTextUtilities();

        public void Setup(string connectionString, string databaseName, string userLoginsCollectionName) {
            this.DbClient = new MongoClient(connectionString);
            this.DbServer = this.DbClient.GetServer();
            this.Database = this.DbServer.GetDatabase(databaseName);
            this.UserLogins = this.Database.GetCollection<DbUserLogin>(userLoginsCollectionName);
        }

        public bool ExistLogin(string login) {
            var query = Query<DbUserLogin>.EQ(user => user.login, login);
            return UserLogins.Count(query) > 0;
        }

        public bool CheckUserLoginAndPassword(string login, string password) {
            var query = Query.And(
                Query<DbUserLogin>.EQ(user => user.login, login),
                Query<DbUserLogin>.EQ(user => user.password, password)
            );
            return this.UserLogins.Count(query) > 0;
        }

        public DbUserLogin GetExistingUser(string login, string password) {
            var query = Query.And(
                Query<DbUserLogin>.EQ(user => user.login, login),
                Query<DbUserLogin>.EQ(user => user.password, password)
            );
            return UserLogins.FindOne(query);
        }

        public DbUserLogin GetExistingUserForGameRef(string login, string gameRef) {
            var query = Query.And(
                Query<DbUserLogin>.EQ(user => user.login, login),
                Query<DbUserLogin>.EQ(user => user.gameRef, gameRef)
            );
            return UserLogins.FindOne(query);
        }

        public DbUserLogin GetExistingUserForGameRefOnly(string gameRef) {
            var query = Query<DbUserLogin>.EQ(user => user.gameRef, gameRef);
            return UserLogins.FindOne(query);
        }

        public DbUserLogin GetExistingUserForEmail(string email) {
            var query = Query<DbUserLogin>.EQ(user => user.email, email);
            return UserLogins.FindOne(query);
        }


        public bool ExistGameRef(string gameRef) {
            var query = Query<DbUserLogin>.EQ(u => u.gameRef, gameRef);
            return this.UserLogins.Count(query) > 0;
        }

        public DbUserLogin CreateUser(string login, string password, string email, out LoginReturnCode code) {
            code = LoginReturnCode.Ok;

            DbUserLogin dbUser = new DbUserLogin {
                creationTime = CommonUtils.SecondsFrom1970(),
                email = email,
                expireTime = CommonUtils.SecondsFrom1970() + NEW_USER_GAME_INTERVAL_IN_SECONDS,
                gameRef = Guid.NewGuid().ToString(),
                login = login,
                passes = 0,
                password = password
            };
            var result = UserLogins.Save(dbUser);
            return dbUser;
        }

        public void SaveUser(DbUserLogin user ) {
            UserLogins.Save(user);
        }

        public DbUserLogin GetExistingUser(string login, string password, out LoginReturnCode code) {
            code = LoginReturnCode.Ok;

            if (!mLoginUtilities.IsLoginCharactersValid(login)) {
                code = LoginReturnCode.LoginHasInvalidCharacters;
                return null;
            }


            if (!mLoginUtilities.IsLoginLengthValid(login)) {
                code = LoginReturnCode.LoginVeryShort;
                return null;
            }

            if(CheckUserLoginAndPassword(login, password)) {
                var user = GetExistingUser(login, password);
                if(user == null ) {
                    code = LoginReturnCode.UserWithLoginAndPasswordNotFound;
                    return null;
                }
                return user;
            } else {
                code = LoginReturnCode.UserLoginOrPasswordIncorrect;
                return null;
            }


/*
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
                
            } */
        }
    }


}
