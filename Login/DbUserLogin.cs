// DbUserLogin.cs
// Nebula
//
// Created by Oleg Zheleztsov on Thursday, November 5, 2015 4:22:34 PM
// Copyright (c) 2015 KomarGames. All rights reserved.
//
using MongoDB.Bson;

namespace Login {

    /// <summary>
    /// Info about user what saved in database
    /// </summary>
    public class DbUserLogin {
        /// <summary>
        /// DB id
        /// </summary>
        public ObjectId Id { get; set; }

        /// <summary>
        /// Login of user
        /// </summary>
        public string login { get; set; } = string.Empty;

        /// <summary>
        /// User password
        /// </summary>
        public string password { get; set; } = string.Empty;

        /// <summary>
        /// User email
        /// </summary>
        public string email { get; set; } = string.Empty;

        /// <summary>
        /// User unique game ref
        /// </summary>
        public string gameRef { get; set; } = string.Empty;

        /// <summary>
        /// User unix creation time
        /// </summary>
        public int creationTime { get; set; } = 0;

        /// <summary>
        /// User facebook id
        /// </summary>
        public string facebookId { get; set; } = string.Empty;

        /// <summary>
        /// User vkontakte id
        /// </summary>
        public string vkontakteId { get; set; } = string.Empty;

        public int nebulaCredits { get; set; } = 0;

        //All info about passes and game time don't applyable now
        /*
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
        }*/

        public void RemoveNebulaCredits(int count) {
            nebulaCredits -= count;
            if(nebulaCredits < 0 ) {
                nebulaCredits = 0;
            }
        }
    }
}
