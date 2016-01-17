using System.Linq;

#if UP
using Nebula.Client.UP;
#else
using System.Xml.Linq;
#endif

namespace Nebula.Client.Pets {
    public class PetPassiveBonusCollection : KeyValueTable<int, PetPassiveBonus> {
        public override void Load(object elementObj) {
#if UP
            var element = elementObj as UPXElement;
#else
            var element = elementObj as XElement;
#endif

            var dump = element.Elements("bonus").Select(bonusElement => {
                PetPassiveBonus bon = new PetPassiveBonus(bonusElement);
                this[bon.id] = bon;
                return bon;
            }).ToList();
        }

        public void LoadText(string xml) {
#if UP
            UPXDocument document = new UPXDocument(xml);
#else
            XDocument document = XDocument.Parse(xml);
#endif
            Load(document.Element("bonuses"));
        }
    }
}
