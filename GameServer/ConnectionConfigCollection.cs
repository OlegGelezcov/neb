using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Common;
using ExitGames.Logging;

namespace Nebula {
    public class ConnectionConfigCollection : Dictionary<string, ConnectionRole> {

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private readonly object syncRoot = new object();

        public void LoadFromFile(string file) {
            lock(syncRoot) {
                try {
                    XDocument document = XDocument.Load(file);
                    var roleArray = document.Element("roles").Elements("role").Select(r => {
                        List<string> locations = new List<string>();
                        if(r.Element("locations") != null ) {
                            locations = r.Element("locations").Value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        }
                        return new ConnectionRole {
                            GamingTcpPort = r.GetInt("gaming_tcp_port"),
                            GamingUdpPort = r.GetInt("gaming_udp_port"),
                            GamingWebSocketPort = r.GetInt("gaming_web_socket_port"),
                            PublicIPAddress = r.GetString("public_ip_address"),
                            RoleName = r.GetString("name").ToLower(),
                            Locations = locations
                        };
                    }).ToArray();

                    foreach (var role in roleArray) {
                        Add(role.RoleName, role);
                    }
                }catch(Exception exception) {
                    log.Error(exception);
                }
            }
        }

        public bool TryGetRoleConnection(string roleName, out ConnectionRole role) {
            lock(syncRoot) {
                return TryGetValue(roleName, out role);
            }
        }
    }
}
