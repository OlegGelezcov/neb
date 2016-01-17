using Nebula.Resources;
using Space.Game;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Nebula.Pets {
    public class PetPassiveBonusCollection : KeyValueTable<int, PetPassiveBonusInfo> {

        public void Load(string file) {
            XDocument document = XDocument.Load(file);
            Load(document.Element("bonuses"));
        }
        public override void Load(XElement element) {
            var dump = element.Elements("bonus").Select(bonusElement => {
                PetPassiveBonusInfo bonusInfo = new PetPassiveBonusInfo(bonusElement);
                this[bonusInfo.id] = bonusInfo;
                return bonusInfo;
            }).ToList();
        }

        public int randomBonus {
            get {
                List<int> ids = new List<int>(dict.Keys);
                return ids.AnyElement();
            }
        }
    }
}
