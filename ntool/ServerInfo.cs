using NebulaCommon;

namespace ntool {
    public class ServerInfo {

        private string m_ServerName;
        private int m_Port;
        private string m_IP;
        private string m_ApplicationName;
        private ServerType m_serverType;


        public ServerInfo(ServerType type, string serverName, string ip, int port, string appName ) {
            m_ServerName = serverName;
            m_Port = port;
            m_IP = ip;
            m_ApplicationName = appName;
            m_serverType = type;
        }

        public string server {
            get {
                return m_ServerName;
            }
        }

        public int port {
            get {
                return m_Port;
            }
        }

        public string ip {
            get {
                return m_IP;
            }
        }

        public string appName {
            get {
                return m_ApplicationName;
            }
        }

        public ServerType type {
            get {
                return m_serverType;
            }
        }
    }
}
