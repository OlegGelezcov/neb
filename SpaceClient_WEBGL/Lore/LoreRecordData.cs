using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Client.Lore {
    public class LoreRecordData {
        public string recordId { get; private set; }
        public int index { get; private set; }

        public LoreRecordData(UniXMLElement element) {
            recordId = element.GetString("id");
            index = element.GetInt("index");
        }

    }
}
