using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Nebula.Resources {
    public class CraftResourceObjectTable : KeyValueTable<string, CraftResourceObjectData> {

        public void Load(string file) {
            XDocument document = XDocument.Load(file);
            Load(document.Element("resources"));
        }
        public override void Load(XElement element) {
            var dump = element.Elements("res").Select(resElement => {
                CraftResourceObjectData data = new CraftResourceObjectData(resElement);
                this[data.id] = data;
                return data;
            }).ToList();
        }

        public List<CraftResourceObjectData> all {
            get {
                List<CraftResourceObjectData> list = new List<CraftResourceObjectData>();
                foreach(var pair in dict) {
                    list.Add(pair.Value);
                }
                return list;
            }
        }
    }
}
