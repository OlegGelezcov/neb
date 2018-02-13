using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServerClientCommon;
using ExitGames.Client.Photon;

namespace Nebula.Client.Quests {
    public class QuestUpdateEventData : IInfoParser {

        private QuestDataCollection dataCollection;

        public PlayerQuestsInfo PlayerQuests { get; private set; }

        public QuestUpdateEventData(QuestDataCollection collection) {
            dataCollection = collection;
        }

        public void ParseInfo(Hashtable info) {
            PlayerQuests = new PlayerQuestsInfo(dataCollection);
            if(info.ContainsKey((int)SPC.Info)) {
                Hashtable hash = info[(int)SPC.Info] as Hashtable;
                if( hash != null ) {
                    PlayerQuests.ParseInfo(hash);
                }
            }
        }
    }
}
