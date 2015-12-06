using ServerClientCommon;
using System.Collections.Generic;
using System.Linq;

namespace Nebula.Client.Servers {
    public class ClientServerCollection : List<ServerInfo> {

        /// <summary>
        /// get first server witth selected type
        /// </summary>
        /// <param name="serverType"></param>
        public ServerInfo GetServer(ServerType serverType) {
            foreach (var server in this) {
                if (server.ServerType == serverType) {
                    return server;
                }
            }
            return null;
        }

        public ServerInfo GetServer(ServerType serverType, int index) {
            foreach (var server in this) {
                if (server.ServerType == serverType && server.Index == index) {
                    return server;
                }
            }
            return null;
        }

        /// <summary>
        /// Check that input server type server contains in collection
        /// </summary>
        /// <param name="serverType">Target server type</param>
        /// <returns>true if contains such server, other false</returns>
        public bool ContainsServer(ServerType serverType) {
            foreach (var server in this) {
                if (server.ServerType == serverType) {
                    return true;
                }
            }
            return false;
        }

        public ServerInfo GetGameServerForLocation(string worldId) {
            foreach (var server in this) {
                if (server.ServerType == ServerType.game) {
                    if (server.Locations != null) {
                        if (server.Locations.Contains(worldId)) {
                            return server;
                        }
                    }
                }
            }
            return null;
        }
    }
}
