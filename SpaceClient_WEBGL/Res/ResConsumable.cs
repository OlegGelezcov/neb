using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Nebula.Client.Res {
    public class ResConsumable {
        public Dictionary<string, ResConsumableInfo> consumables { get; private set; }

        public ResConsumable() {
            consumables = new Dictionary<string, ResConsumableInfo>();
        }

        public void Load(string xml) {
            XDocument document = XDocument.Parse(xml);
            consumables = document.Element("items").Elements("item").Select(itemElement => {
                var cons = new ResConsumableInfo(itemElement);
                return cons;
            }).ToDictionary(c => c.id, c => c);
        }

        public ResConsumableInfo GetConsumableInfo(string id) {
            if(consumables.ContainsKey(id)) {
                return consumables[id];
            }
            return null;
        }
    }
}
