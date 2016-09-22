using System.Collections.Generic;
using System.Linq;

namespace Nebula.Client.Manual {
    public class ManQuestData {
        public string id { get; private set; }
        //public ManQuestCategory type { get; private set; }
        public string name { get; private set; }
        public string startText { get; private set; }
        public string completeText { get; private set; }
        public List<ManStageData> stages { get; private set; }
        public int index { get; private set; }
        public List<string> prevQuests { get; private set; }

        public ManQuestData(UniXMLElement element ) {
            id = element.GetString("id");
            //type = (ManQuestCategory)System.Enum.Parse(typeof(ManQuestCategory), element.GetString("type"));
            name = element.GetString("name");
            startText = element.GetString("start_text");
            completeText = element.GetString("complete_text");
            index = element.GetInt("index");

            prevQuests = new List<string>();
            string prevQuestString = element.GetString("prev_quests");
            string[] tokens = prevQuestString.Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);

            if (tokens.Length > 0 ) {
                prevQuests.AddRange(tokens);
            }

            stages = element.Elements("stage").Select(se => {
                return new ManStageData(se);
            }).ToList();
        }

        public ManStageData GetStage(int stage) {
            foreach(var s in stages) {
                if(s.stage == stage ) {
                    return s;
                }
            }
            return null;
        }
    }
}
