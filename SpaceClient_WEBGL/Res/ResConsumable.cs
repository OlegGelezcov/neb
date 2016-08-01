using Common;
using System.Collections.Generic;
using System.Linq;
#if UP
using Nebula.Client.UP;
#else
using System.Xml.Linq;
#endif

namespace Nebula.Client.Res {
    public class ResConsumable {
        public Dictionary<string, ResConsumableInfo> consumables { get; private set; }

        public ResConsumable() {
            consumables = new Dictionary<string, ResConsumableInfo>();
        }

        public void Load(string xml) {
#if UP
            UPXDocument document = new UPXDocument(xml);
#else
            XDocument document = XDocument.Parse(xml);
#endif
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
