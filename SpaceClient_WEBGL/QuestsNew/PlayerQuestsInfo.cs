using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServerClientCommon;
using ExitGames.Client.Photon;

namespace Nebula.Client.Quests {
    public class PlayerQuestsInfo : IInfoParser {

        public List<string> CompletedQuests { get; private set; } = new List<string>();
        public List<QuestInfo> StartedQuests { get; private set; } = new List<QuestInfo>();
        public Dictionary<string, object> QuestVariables { get; private set; } = new Dictionary<string, object>();

        private QuestDataCollection dataCollection = null;

        public override string ToString() {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"Completed => {string.Join(",", CompletedQuests.ToArray())}");
            builder.AppendLine("Started => ");
            StartedQuests.ForEach(q => {
                builder.AppendLine(q.ToString());
            });
            builder.AppendLine("Quest variables => ");
            foreach(var pair in QuestVariables) {
                builder.AppendLine($"{pair.Key} => {pair.Value}");
            }
            return builder.ToString();
        }

        public PlayerQuestsInfo(QuestDataCollection collection) {
            dataCollection = collection;
        }

        public void ParseInfo(Hashtable info) {
            CompletedQuests.Clear();
            if(info.ContainsKey((int)SPC.CompletedQuests)) {
                string[] arr = info[(int)SPC.CompletedQuests] as string[];
                CompletedQuests.AddRange(arr);
            }

            StartedQuests.Clear();
            if(info.ContainsKey((int)SPC.StartedQuests)) {
                Hashtable[] hashArr = info[(int)SPC.StartedQuests] as Hashtable[];
                if(hashArr != null ) {
                    foreach(Hashtable questHash in hashArr ) {
                        QuestInfo quest = new QuestInfo(dataCollection);
                        quest.ParseInfo(questHash);
                        StartedQuests.Add(quest);
                    }
                }
            }

            QuestVariables.Clear();
            if(info.ContainsKey((int)SPC.Variables)) {
                Hashtable varHash = info[(int)SPC.Variables] as Hashtable;
                if(varHash != null ) {
                    foreach(System.Collections.DictionaryEntry entry in varHash ) {
                        QuestVariables.Add(entry.Key.ToString(), entry.Value);
                    }
                }
            }
        }

        public bool HasStartedQuests{
            get {
                return StartedQuests.Count > 0;
            }
        }

        public QuestInfo FirstStartedQuest {
            get {
                if(HasStartedQuests ) {
                    return StartedQuests[0];
                }
                return null;
            }
        }
    }
}
