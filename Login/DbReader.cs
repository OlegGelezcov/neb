// DbReader.cs
// Nebula
//
// Created by Oleg Zheleztsov on Thursday, November 5, 2015 4:23:01 PM
// Copyright (c) 2015 KomarGames. All rights reserved.
//
using Common;
using ExitGames.Logging;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Nebula.Server.Login;
using ServerClientCommon;
using System;
using System.Collections.Generic;

namespace Login {

    /// <summary>
    /// Database interface for users
    /// </summary>
    public class DbReader {

        public const int NEBULA_CREDITS_AT_START = 1000;
        /// <summary>
        /// Client
        /// </summary>
        public MongoClient DbClient { get; private set; }

        /// <summary>
        /// Database server
        /// </summary>
        public MongoServer DbServer { get; private set; }

        /// <summary>
        /// Database
        /// </summary>
        public MongoDatabase Database { get; private set; }

        /// <summary>
        /// User collection
        /// </summary>
        public MongoCollection<DbUserLogin> UserLogins { get; private set; }

        public MongoCollection<DbUserStat> UserStats { get; private set;  }

        /// <summary>
        /// Logger
        /// </summary>
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// String utils object
        /// </summary>
        private readonly LoginTextUtilities mLoginUtilities = new LoginTextUtilities();


        /// <summary>
        /// Set database connection
        /// </summary>
        /// <param name="connectionString">Database connection string</param>
        /// <param name="databaseName">Database name where users saved</param>
        /// <param name="userLoginsCollectionName">Users collection name in Mongo Database</param>
        public void Setup(string connectionString, string databaseName, string userLoginsCollectionName) {
            this.DbClient = new MongoClient(connectionString);
            this.DbServer = this.DbClient.GetServer();
            this.Database = this.DbServer.GetDatabase(databaseName);

            this.UserLogins = this.Database.GetCollection<DbUserLogin>(userLoginsCollectionName);
            this.UserStats = this.Database.GetCollection<DbUserStat>("userstats");
        }

        /// <summary>
        /// Check exists or not user with login
        /// </summary>
        /// <param name="inlogin">Test login</param>
        /// <returns>TRUE if such user exists in db, FALSE - if user dont exists in database</returns>
        public bool ExistsUser(LoginId inLogin) {
            //user logins case independent
            string login = inLogin.value;
            var query = Query<DbUserLogin>.EQ(user => user.login, login);
            return UserLogins.Count(query) > 0;
        }

        /// <summary>
        /// Check existence in database user with facebook id
        /// </summary>
        public bool ExistsUser(FacebookId inFacebook) {
            string fbId = inFacebook.value;
            var query = Query<DbUserLogin>.EQ(user => user.facebookId, fbId);
            return UserLogins.Count(query) > 0;
        }

        /// <summary>
        /// Check existence in database user with vkontakte id
        /// </summary>
        public bool ExistsUser(VkontakteId inVkontakte) {
            string vkId = inVkontakte.value;
            var query = Query<DbUserLogin>.EQ(user => user.vkontakteId, vkId);
            return UserLogins.Count(query) > 0;
        }

        /// <summary>
        /// Check existence user with login and password
        /// </summary>
        public bool ExistsUser(LoginAuth auth) {

            string login = auth.login;
            string password = auth.password;

            var query = Query.And(
                Query<DbUserLogin>.EQ(user => user.login, login),
                Query<DbUserLogin>.EQ(user => user.password, password)
            );
            return this.UserLogins.Count(query) > 0;
        }

        public bool ExistsUser(GameRefId inGameRef) {
            string gameRef = inGameRef.value;
            var query = Query<DbUserLogin>.EQ(u => u.gameRef, gameRef);
            return this.UserLogins.Count(query) > 0;
        }

        public DbUserStat GetStat(string login) {
            var query = Query<DbUserStat>.EQ(user => user.login, login);
            var stat = UserStats.FindOne(query);
            if(stat == null ) {
                stat = new DbUserStat {
                    login = login,
                    sessions = new List<int>(),
                    platform = string.Empty
                };
            }
            return stat;
        }

        public void SaveStat(DbUserStat stat) {
            UserStats.Save(stat);
        }


        /// <summary>
        /// Get from database user with permissions
        /// </summary>
        /// <param name="auth">Authentication object</param>
        /// <returns>Founded user or null</returns>
        public DbUserLogin GetUser(LoginAuth auth) {
            string login = auth.login;
            string password = auth.password;

            var query = Query.And(
                Query<DbUserLogin>.EQ(user => user.login, login),
                Query<DbUserLogin>.EQ(user => user.password, password)
            );
            return UserLogins.FindOne(query);
        }

        public DbUserLogin GetUser(LoginGameRef auth) {
            string login = auth.login;
            string gameRef = auth.gameRef;

            var query = Query.And(
                Query<DbUserLogin>.EQ(user => user.login, login),
                Query<DbUserLogin>.EQ(user => user.gameRef, gameRef)
            );
            return UserLogins.FindOne(query);
        }

        public DbUserLogin GetUser(GameRefId inGameRef) {
            string gameRef = inGameRef.value;
            var query = Query<DbUserLogin>.EQ(user => user.gameRef, gameRef);
            return UserLogins.FindOne(query);
        }

        public DbUserLogin GetUser(Email mail) {
            string email = mail.value;
            var query = Query<DbUserLogin>.EQ(user => user.email, email);
            return UserLogins.FindOne(query);
        }

        public DbUserLogin GetUser(FacebookId fbId ) {
            string facebookId = fbId.value;
            var query = Query<DbUserLogin>.EQ(user => user.facebookId, facebookId);
            return UserLogins.FindOne(query);
        }

        public DbUserLogin GetUser(DeviceId deviceId ) {
            string value = deviceId.Value;
            var query = Query<DbUserLogin>.EQ(user => user.deviceId, value);
            return UserLogins.FindOne(query);
        }

        public DbUserLogin GetUser(SteamId steamId ) {
            string sid = steamId.value;
            var query = Query<DbUserLogin>.EQ(user => user.steamId, sid);
            return UserLogins.FindOne(query);
        }

        public DbUserLogin GetUser(VkontakteId vkId ) {
            string vkontakteId = vkId.value;
            var query = Query<DbUserLogin>.EQ(user => user.vkontakteId, vkontakteId);
            return UserLogins.FindOne(query);
        }




        public DbUserLogin CreateUser(LoginAuth auth, Email email, FacebookId fbId, VkontakteId vkId) {

            DbUserLogin dbUser = new DbUserLogin {
                creationTime = CommonUtils.SecondsFrom1970(),
                email = email.value,
                gameRef = Guid.NewGuid().ToString(),
                login = auth.login,
                password = auth.password,
                facebookId = fbId.value,
                vkontakteId = vkId.value,
                nebulaCredits = NEBULA_CREDITS_AT_START,
                steamId = string.Empty,
                deviceId = string.Empty
            };

            var result = UserLogins.Save(dbUser);

            return dbUser;
        }

        public DbUserLogin CreateUser(FacebookId facebookId ) {
            DbUserLogin databaseUser = new DbUserLogin {
                creationTime = CommonUtils.SecondsFrom1970(),
                email = string.Empty,
                gameRef = Guid.NewGuid().ToString(),
                login = facebookId.value,
                password = facebookId.value,
                facebookId = facebookId.value,
                vkontakteId = string.Empty,
                nebulaCredits = NEBULA_CREDITS_AT_START,
                steamId = string.Empty,
                deviceId = string.Empty
            };
            var result = UserLogins.Save(databaseUser);
            return databaseUser;
        }

        public DbUserLogin CreateUser(DeviceId deviceId ) {
            DbUserLogin databaseUser = new DbUserLogin {
                creationTime = CommonUtils.SecondsFrom1970(),
                email = string.Empty,
                gameRef = Guid.NewGuid().ToString(),
                login = deviceId.Value,
                password = deviceId.Value,
                facebookId = string.Empty,
                vkontakteId = string.Empty,
                nebulaCredits = NEBULA_CREDITS_AT_START,
                steamId = string.Empty,
                deviceId = deviceId.Value
            };
            var result = UserLogins.Save(databaseUser);
            return databaseUser;
        }

        public DbUserLogin CreateUser(SteamId steamId ) {
            DbUserLogin databaseUser = new DbUserLogin {
                creationTime = CommonUtils.SecondsFrom1970(),
                email = string.Empty,
                gameRef = Guid.NewGuid().ToString(),
                login = steamId.value,
                password = steamId.value,
                facebookId = string.Empty,
                vkontakteId = string.Empty,
                nebulaCredits = NEBULA_CREDITS_AT_START,
                steamId = steamId.value,
                deviceId = string.Empty
            };
            var result = UserLogins.Save(databaseUser);
            return databaseUser;
        }

        public DbUserLogin CreateUser(VkontakteId vkontakteId ) {
            DbUserLogin databaseUser = new DbUserLogin {
                creationTime = CommonUtils.SecondsFrom1970(),
                email = string.Empty,
                gameRef = Guid.NewGuid().ToString(),
                login = vkontakteId.value,
                password = vkontakteId.value,
                facebookId = string.Empty,
                vkontakteId = vkontakteId.value,
                nebulaCredits = 0,
                steamId = string.Empty,
                deviceId = string.Empty
            };
            var result = UserLogins.Save(databaseUser);
            return databaseUser;
        }

        public void SaveUser(DbUserLogin user ) {
            UserLogins.Save(user);
        }
    }
}
