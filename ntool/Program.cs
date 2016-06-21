//#define LOCAL
using System;
using System.Collections.Generic;

namespace ntool {
    class Program {
        static void Main(string[] args) {
#if LOCAL
            List<ServerInfo> servers = new List<ServerInfo> {
                new ServerInfo(NebulaCommon.ServerType.Master, "master", "192.168.1.102", 5105, "Master"),
                new ServerInfo(NebulaCommon.ServerType.Login, "login", "192.168.1.102", 5107, "Login" ),
                new ServerInfo(NebulaCommon.ServerType.Game, "game", "192.168.1.102", 5106, "HumansGame"),
                new ServerInfo(NebulaCommon.ServerType.SelectCharacter,"select-character", "192.168.1.102", 5108, "SelectCharacter")
            };
#else
            List<ServerInfo> servers = new List<ServerInfo> {
                new ServerInfo(NebulaCommon.ServerType.Master, "master", "45.63.0.198", 5105, "Master"),
                new ServerInfo(NebulaCommon.ServerType.Login, "login", "45.63.0.198", 5107, "Login" ),
                new ServerInfo(NebulaCommon.ServerType.Game, "game", "45.63.19.226", 5106, "HumansGame"),
                new ServerInfo(NebulaCommon.ServerType.SelectCharacter,"select-character", "45.63.0.198", 5108, "SelectCharacter")
            };
#endif
            Application.instance.Connect(servers);
            Application.instance.Run();
        }
    }
}
