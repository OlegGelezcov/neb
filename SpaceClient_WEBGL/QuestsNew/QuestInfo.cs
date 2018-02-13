using Common;
using ExitGames.Client.Photon;
using Nebula.Client.Utils;
using ServerClientCommon;

namespace Nebula.Client.Quests {
    public class QuestInfo : IInfoParser {

        private QuestDataCollection dataCollection = null;

        public string Id { get; private set; }
        public int Count { get; private set; }
        public QuestState State { get; private set; }

        private QuestData data = null;

        public QuestData Data {
            get {
                if(data == null ) {
                    data = dataCollection.GetQuestData(Id);
                }
                return data;
            }
        }

        public QuestInfo(QuestDataCollection collection) {
            dataCollection = collection;
        }

        #region IInfoParser
        public void ParseInfo(Hashtable info) {
            Id = info.GetValueString((int)SPC.Id);
            Count = info.GetValueInt((int)SPC.Counter);
            if(info.ContainsKey((int)SPC.State)) {
                State = (QuestState)(byte)(int)info[(int)SPC.State];
            }
            data = dataCollection.GetQuestData(Id);
        }
        #endregion

        public override string ToString() {
            return $"Id => {Id}, Count => {Count}, State => {State}";
        }

        public string GetProgressString(Race race) {
            if(Data.HasCondition<NpcKilledWithLevelQuestCondition>(race) ||
                Data.HasCondition<NpcKilledWithColorQuestCondition>(race) ||
                Data.HasCondition<NpcKilledWithClassQuestCondition>(race) ||
                Data.HasCondition<CollectOreQuestCondition>(race) ||
                Data.HasCondition<ModuleCraftedQuestCondition>(race) ||
                Data.HasCondition<CreateStructureQuestCondition>(race) ||
                Data.HasCondition<NpcKilledWithBotGroupQuestCondition>(race)) {
                return $"{Count}/{Data.CompleteConditions[race].Repeat}";
            }
            return string.Empty;
        }
    }
}
