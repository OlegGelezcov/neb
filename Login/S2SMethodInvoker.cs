using Common;
using ExitGames.Logging;
using Login.Events;
using Nebula.Server.Login;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Login {
    public class S2SMethodInvoker {
        private const string LOG_TAG = "S2SMETHOD";
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        private LoginApplication application;

        public S2SMethodInvoker(LoginApplication application ) {
            this.application = application;
        }

        public object AddNebulaCredits(string login, string gameRefId, string characterId, int nebulaCredits ) {
            logger.Info($"{LOG_TAG} => AddNebulaCredits({login}, {gameRefId}, {characterId}, {nebulaCredits})");
            GameRefId gameRef = new GameRefId(gameRefId);
            DbUserLogin user = application.GetUser(gameRef);
            if(user == null ) {
                return (int)ReturnCode.Fatal;
            }
            user.nebulaCredits += nebulaCredits;
            if(user.nebulaCredits < 0 ) {
                user.nebulaCredits = 0;
            }
            application.SaveUser(user);
            application.LogedInUsers.SendEvent(gameRef, new Photon.SocketServer.EventData((byte)LoginEventCode.NebulaCreditsUpdate, new NebulaCreditsUpdateEvent { nebulaCredits = user.nebulaCredits }));
            return (int)ReturnCode.Ok;
        }
    }
}
