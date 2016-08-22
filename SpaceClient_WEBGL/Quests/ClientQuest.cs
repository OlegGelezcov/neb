using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExitGames.Client.Photon;
using Nebula.Client.Utils;
using ServerClientCommon;

namespace Nebula.Client.Quests {
    public class ClientQuest : IInfoParser {

        private readonly ClientQuestVariableCollection m_Variables = new ClientQuestVariableCollection();
        private string m_Id;
        private ServerQuestState m_State;

        public ClientQuest(Hashtable hash) {
            ParseInfo(hash);
        }

        public void ParseInfo(Hashtable info) {
            m_Id = info.GetValueString((int)SPC.Id);
            m_State = (ServerQuestState)info.GetValueInt((int)SPC.State);

            Hashtable varHash = info.GetValueHash((int)SPC.Variables);
            if(varHash != null ) {
                m_Variables.Load(varHash);
            } else {
                m_Variables.Clear();
            }
        }

        public string id {
            get {
                return m_Id;
            }
        }

        public ServerQuestState state {
            get {
                return m_State;
            }
        }

        public ClientQuestVariableCollection variables {
            get {
                return m_Variables;
            }
        }
    }
}
