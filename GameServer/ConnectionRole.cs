using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula {
    public class ConnectionRole {
        public string RoleName;
        public int GamingTcpPort;
        public int GamingUdpPort;
        public int GamingWebSocketPort;
        public string PublicIPAddress;
        public List<string> Locations;
    }


}
