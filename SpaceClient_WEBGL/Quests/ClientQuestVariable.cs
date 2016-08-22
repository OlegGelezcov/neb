using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExitGames.Client.Photon;
using Nebula.Client.Utils;
using ServerClientCommon;

namespace Nebula.Client.Quests {
    public class ClientQuestVariable : IInfoParser {

        private string m_Name;
        private string m_Type;

        public virtual void ParseInfo(Hashtable info) {
            m_Name = info.GetValueString((int)SPC.Name);
            m_Type = info.GetValueString((int)SPC.Type);
        }

        public string name {
            get {
                return m_Name;
            }
        }

        public string type {
            get {
                return m_Type;
            }
        }
    }
}
