using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Server.Login {
    public class SteamId {

        private string m_SteamId;

        public SteamId(string steamId) {
            m_SteamId = steamId;
        }

        public string value {
            get {
                if(m_SteamId == null ) {
                    m_SteamId = string.Empty;
                }
                return m_SteamId;
            }
        }

        public override string ToString() {
            return value;
        }
    }
}
