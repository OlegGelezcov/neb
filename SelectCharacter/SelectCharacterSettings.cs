﻿namespace SelectCharacter {
    public class SelectCharacterSettings {

        public int GamingTcpPort;
        public int GamingUdpPort;
        public int GamingWebSocketPort;
        public int ConnectRetryInterval;
        public string PublicIPAddress;
        public string MasterIPAddress;
        public int OutgoingMasterServerPeerPort;
        //public string DatabaseConnectionString;
        public string DatabaseName;
        public string DatabaseCollectionName;
        public int MaxPlayerCharactersCount;
        public int DatabaseSaveInterval;

        public static SelectCharacterSettings Default {
            get {
#if LOCAL
                return new SelectCharacterSettings {
                    GamingTcpPort = 4563,
                    GamingUdpPort = 5108,
                    GamingWebSocketPort = 9093,
                    ConnectRetryInterval = 15,
                    PublicIPAddress = "192.168.1.8",
                    MasterIPAddress = "192.168.1.8",
                    OutgoingMasterServerPeerPort = 4520,
                    DatabaseCollectionName = "character_collection",
                    MaxPlayerCharactersCount = 1,
                    DatabaseName = "characters",
                    DatabaseSaveInterval = 6
                };
#else
                return new SelectCharacterSettings {
                    GamingTcpPort = 4563,
                    GamingUdpPort = 5108,
                    GamingWebSocketPort = 9093,
                    ConnectRetryInterval = 15,
                    PublicIPAddress = "108.61.87.145",
                    MasterIPAddress = "108.61.87.145",
                    OutgoingMasterServerPeerPort = 4520,
                    DatabaseCollectionName = "character_collection",
                    MaxPlayerCharactersCount = 1,
                    DatabaseName = "characters",
                    DatabaseSaveInterval = 6
                };
#endif
            }
        }
    }
}
