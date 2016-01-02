// MethodInvoker.cs
// Nebula
//
// Created by Oleg Zheleztsov on Thursday, November 5, 2015 4:21:24 PM
// Copyright (c) 2015 KomarGames. All rights reserved.
//
using Common;
using ExitGames.Logging;
using Nebula.Server.Login;
using ServerClientCommon;
using System.Collections;

namespace Login {
    public class MethodInvoker {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public LoginApplication application { get; private set; }
        public LoginClientPeer peer { get; private set; }

        public MethodInvoker(LoginApplication app, LoginClientPeer impeer) {
            application = app;
            peer = impeer;
        }

        /// <summary>
        /// Get newbula credits count for game ref
        /// </summary>
        /// <param name="gameRefId">Gameref of user for lookup account</param>
        /// <returns>Number of credits of user</returns>
        public int GetNebulaCredits(string gameRefId) {
            GameRefId gameRef = new GameRefId(gameRefId);
            var user = application.GetUser(gameRef);
            if(user != null ) {
                return user.nebulaCredits;
            } else {
                return 0;
            }
        }

        /// <summary>
        /// Add nebula credits to user account and save user in database
        /// </summary>
        /// <param name="gameRefId">Gameref of user</param>
        /// <param name="credits">Number of credits to add (might be less than zero, than credits will be clamped to zero)</param>
        /// <returns>Status of operation</returns>
        public bool AddNebulaCredits(string gameRefId, int credits) {
            GameRefId gameRef = new GameRefId(gameRefId);
            var user = application.GetUser(gameRef);
            if(user == null ) {
                return false;
            }

            user.nebulaCredits += credits;
            if(user.nebulaCredits < 0 ) {
                user.nebulaCredits = 0;
            }
            application.SaveUser(user);
            return true;
        }

        /// <summary>
        /// Request purchase item
        /// </summary>
        /// <param name="gameRef">Game ref of player</param>
        /// <param name="character">ID of character </param>
        /// <param name="inapId">ID of inap</param>
        /// <param name="targetServer">Server where mail service plaiced</param>
        /// <returns></returns>
        public Hashtable RequestPurchaseInap(string gameRef, string character, string inapId, string targetServer) {
            ReturnCode code;
            bool success = application.inaps.RequestPurchaseInap(gameRef, character, inapId, targetServer, out code);
            return new Hashtable {
                { (int)SPC.Status, success },
                { (int)SPC.ReturnCode, (int)code }
            };
        }
    }
}
