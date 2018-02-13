using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExitGames.Client.Photon;
using ServerClientCommon;

namespace Nebula.Client.Quests {
    public class QuestChangedEventData : IInfoParser {

        private QuestDataCollection dataCollection;

        public QuestInfo NewQuest { get; private set; }
        public PlayerQuestsInfo PlayerQuests { get; private set; }

        public QuestChangedEventData(QuestDataCollection collection) {
            dataCollection = collection;
        }

        public void ParseInfo(Hashtable info) {
            NewQuest = new QuestInfo(dataCollection);
            if (info.ContainsKey((int)SPC.NewQuest)) {
                Hashtable hash = info[(int)SPC.NewQuest] as Hashtable;
                if (hash != null) {
                    NewQuest.ParseInfo(hash);
                }
            }

            PlayerQuests = new PlayerQuestsInfo(dataCollection);
            if(info.ContainsKey((int)SPC.Info)) {
                Hashtable hash = info[(int)SPC.Info] as Hashtable;
                if(hash != null ) {
                    PlayerQuests.ParseInfo(hash);
                }
            }
        }
    }
}
