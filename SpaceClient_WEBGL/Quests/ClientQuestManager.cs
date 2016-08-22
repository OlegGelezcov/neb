using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExitGames.Client.Photon;
using Nebula.Client.Utils;
using ServerClientCommon;

namespace Nebula.Client.Quests {
    public class ClientQuestManager : IInfoParser {

        private readonly List<string> m_CompletedQuests = new List<string>();
        private Dictionary<string, ClientQuest> m_ActiveQuests = new Dictionary<string, ClientQuest>();

        public void ParseInfo(Hashtable hash) {
            string[] completedqsts = hash.GetValueStringArray((int)SPC.CompletedQuests);
            m_CompletedQuests.Clear();
            foreach(string quest in completedqsts) {
                m_CompletedQuests.Add(quest);
            }

            m_ActiveQuests.Clear();
            Hashtable questHash = hash.GetValueHash((int)SPC.ActiveQuests);
            if(questHash != null ) {
                foreach(System.Collections.DictionaryEntry kvp in questHash ) {
                    string id = (string)kvp.Key;
                    Hashtable qhash = kvp.Value as Hashtable;
                    if(qhash != null ) {
                        m_ActiveQuests.Add(id, new ClientQuest(qhash));
                    }
                }
            }
        }

        public void Clear() {
            m_CompletedQuests.Clear();
            m_ActiveQuests.Clear();
        }

        public bool IsCompleted(string id) {
            return m_CompletedQuests.Contains(id);
        }

        public bool IsNotCompleted(string id) {
            return (false == IsCompleted(id));
        }

        public bool SetQuestAccepted(ClientQuest quest) {
            if(IsNotCompleted(quest.id)) {
                if(m_ActiveQuests.ContainsKey(quest.id)) {
                    m_ActiveQuests[quest.id] = quest;
                } else {
                    m_ActiveQuests.Add(quest.id, quest);
                }
                return true;
            }
            return false;
        }

        public bool SetQuestCompleted(string questId ) {
            if(IsNotCompleted(questId)) {
                if(m_ActiveQuests.ContainsKey(questId)) {
                    m_ActiveQuests.Remove(questId);
                }
                m_CompletedQuests.Add(questId);
                return true;
            }
            return false;
        }

        public bool SetQuestCompleted(ClientQuest quest) {
            return SetQuestCompleted(quest.id);
        }

        public bool SetQuestReady(ClientQuest quest) {
            if(IsNotCompleted(quest.id)) {
                if(m_ActiveQuests.ContainsKey(quest.id)) {
                    m_ActiveQuests[quest.id] = quest;
                    return true;
                }
            }
            return false;
        }

        public List<ClientQuest> activeQuests {
            get {
                List<ClientQuest> aqList = new List<ClientQuest>(m_ActiveQuests.Values);
                aqList.Sort((x, y) => {
                    return x.id.CompareTo(y.id);
                });
                return aqList;
            }
        }

        public bool IsQuestActiveAndAccepted(string questId ) {
            if(m_ActiveQuests.ContainsKey(questId)) {
                return (m_ActiveQuests[questId].state == ServerQuestState.accepted);
            }
            return false;
        }
    }
}
