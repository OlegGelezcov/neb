﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Login {
    public class GameServerSettings {

        public string MasterIPAddress;
        public int GamingTcpPort;
        public int GamingUdpPort;
        public int ConnectRetryInterval;
        public int OutgoingMasterServerPeerPort;
        public string LatencyMonitorAddress;
        public int RelayPortTcp;
        public int RelayPortWebSocket;
        public string WorkloadConfigFile;
        public string PublicIPAddress;
        public string LatencyMonitorAddressUdp;
        public int RelayPortUdp;
        public bool EnableLatencyMonitor;
        public int GamingWebSocketPort;
        public int AppStatsPublishInterval;
        public string DbLoginCollectionName;
        public string DatabaseName;
        public Assets assets;

        public static GameServerSettings Default {
            get {
#if LOCAL
                return new GameServerSettings {
                    MasterIPAddress = "192.168.1.8",
                    GamingTcpPort = 4562,
                    GamingUdpPort = 5107,
                    ConnectRetryInterval = 15,
                    OutgoingMasterServerPeerPort = 4520,
                   LatencyMonitorAddress = "",
                    RelayPortTcp = 0,
                    RelayPortWebSocket = 0,
                    WorkloadConfigFile = "192.168.1.8",
                    PublicIPAddress = "192.168.1.8",
                    LatencyMonitorAddressUdp = "",
                    RelayPortUdp = 0,
                    EnableLatencyMonitor = false,
                    GamingWebSocketPort = 9092,
                    AppStatsPublishInterval = 1000,
                    DbLoginCollectionName = "user_login_collection",
                    DatabaseName = "user_logins",
                    assets = new Assets()
                };
#else
                return new GameServerSettings {
                    MasterIPAddress = "108.61.87.145",
                    GamingTcpPort = 4562,
                    GamingUdpPort = 5107,
                    ConnectRetryInterval = 15,
                    OutgoingMasterServerPeerPort = 4520,
                    LatencyMonitorAddress = "",
                    RelayPortTcp = 0,
                    RelayPortWebSocket = 0,
                    WorkloadConfigFile = "108.61.87.145",
                    PublicIPAddress = "108.61.87.145",
                    LatencyMonitorAddressUdp = "",
                    RelayPortUdp = 0,
                    EnableLatencyMonitor = false,
                    GamingWebSocketPort = 9092,
                    AppStatsPublishInterval = 1000,
                    DbLoginCollectionName = "user_login_collection",
                    DatabaseName = "user_logins",
                    assets = new Assets()
                };
#endif
            }
        }

        public class Assets {
            public readonly string SERVER_INPUTS_FILE = "Data/server_inputs.xml";
        }
    }
}
