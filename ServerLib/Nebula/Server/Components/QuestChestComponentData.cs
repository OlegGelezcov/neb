using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using System.Xml.Linq;

namespace Nebula.Server.Components {
    public class QuestChestComponentData : ComponentData {

        public List<string> quests;

        public QuestChestComponentData(XElement element ) {
            if (element.HasAttribute("quests")) {
                string questsString = element.GetString("quests");
                if(questsString != null ) {
                    quests = questsString.ToList();
                } else {
                    quests = new List<string>();
                }
            }
            if(quests == null ) {
                quests = new List<string>();
            }
        }

        public override ComponentID componentID {
            get {
                return ComponentID.QuestChest;
            }
        }
    }
}
