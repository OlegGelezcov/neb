// MethodInvoker.cs
// Nebula
//
// Created by Oleg Zheleztsov on Thursday, November 5, 2015 4:21:24 PM
// Copyright (c) 2015 KomarGames. All rights reserved.
//
using ExitGames.Logging;
using ServerClientCommon;

namespace Login {
    public class MethodInvoker {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public LoginApplication application { get; private set; }
        public LoginClientPeer peer { get; private set; }

        public MethodInvoker(LoginApplication app, LoginClientPeer impeer) {
            application = app;
            peer = impeer;
        }

    }
}
