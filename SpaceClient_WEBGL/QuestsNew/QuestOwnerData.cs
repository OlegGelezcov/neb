using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Client.Quests {
    public class QuestOwnerData {
        public string Id { get; private set; }
        public string Icon { get; private set; }
        public string Name { get; private set; }

        public void Load(UniXMLElement element) {
            Id = element.GetString("id");
            Icon = element.GetString("icon");
            Name = element.GetString("name");
        }
    }
}
