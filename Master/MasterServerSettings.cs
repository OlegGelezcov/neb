// MasterServerSettings.cs
// Nebula
//
// Created by Oleg Zheleztsov on Sunday, September 13, 2015 5:43:06 PM
// Copyright (c) 2015 KomarGames. All rights reserved.
//
namespace Master {
    public class MasterServerSettings {
        public int IncomingGameServerPeerPort;
        public bool EnableProxyConnections;
        public int MasterRelayPortTcp;
        public int MasterRelayPortWebSocket;
        public string PublicIPAddress;
        public int MasterRelayPortUdp;
        public int MasterNodeId;
        public string S2SMasterAddress;
        public string DatabaseName;
        public string NewsCollectionName;

        public static MasterServerSettings Default {
            get {
#if LOCAL
                return new MasterServerSettings {
                    IncomingGameServerPeerPort = 4520,
                    EnableProxyConnections = false,
                    MasterNodeId = 1,
                    MasterRelayPortTcp = 0,
                    MasterRelayPortUdp = 0,
                    MasterRelayPortWebSocket = 0,
                    PublicIPAddress = "192.168.1.8",
                    S2SMasterAddress = "192.168.1.8",
                    DatabaseName = "master",
                    NewsCollectionName = "news"
                };
#else
                return new MasterServerSettings {
                    IncomingGameServerPeerPort = 4520,
                    EnableProxyConnections = false,
                    MasterNodeId = 1,
                    MasterRelayPortTcp = 0,
                    MasterRelayPortUdp = 0,
                    MasterRelayPortWebSocket = 0,
                    PublicIPAddress = "108.61.87.145",
                    S2SMasterAddress = "108.61.87.145",
                    DatabaseName = "master",
                    NewsCollectionName = "news"
                };
#endif
            }
        }
    }
}
