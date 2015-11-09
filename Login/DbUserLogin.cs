// DbUserLogin.cs
// Nebula
//
// Created by Oleg Zheleztsov on Thursday, November 5, 2015 4:22:34 PM
// Copyright (c) 2015 KomarGames. All rights reserved.
//
using Common;
using MongoDB.Bson;

namespace Login {
    public class DbUserLogin {
        public ObjectId Id { get; set; }
        public string login { get; set; }
        public string password { get; set; }
        public string email { get; set; }
        public string gameRef { get; set; }
        public int creationTime { get; set; }
        public int passes { get; set; }
        public int expireTime { get; set; }

        public void MoveSinglePassToTime() {
            passes--;
            expireTime += LoginApplication.Instance.serverSettings.timeForPass;
        }

        public void IncrementPasses() {
            passes++;
        }

        public void DecrementPasses() {
            passes--;
        }

        /// <summary>
        /// Force expire game time on user ( for testing )
        /// </summary>
        public void ForceExpire() {
            expireTime = CommonUtils.SecondsFrom1970();
        }
    }
}
