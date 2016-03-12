using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Client.Lore {
    public class LoreStoryData {
        public string name { get; private set; }
        public List<LoreRecordData> records { get; private set; }

        public LoreStoryData(UniXMLElement element) {
            name = element.GetString("name");
            records = element.Elements("record").Select(e => {
                return new LoreRecordData(e);
            }).ToList();
        }

        public LoreRecordData GetRecord(string id) {
            foreach(var rec in records ) {
                if(rec.recordId == id) {
                    return rec;
                }
            }
            return null;
        }
    }
}
