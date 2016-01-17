using Nebula.Resources;
using System.Linq;
using System.Xml.Linq;

namespace Nebula.Pets {
    public class PetMasteryUpgradeTable : KeyValueTable<int, PetMasteryUpgradeRequirement> {
        public override void Load(XElement element) {
            var dump = element.Elements("upgrade").Select(ue => {
                PetMasteryUpgradeRequirement req = new PetMasteryUpgradeRequirement(ue);
                this[req.mastery] = req;
                return req;
            }).ToList();
        }

        public PetMasteryUpgradeTable(XElement element) {
            Load(element);
        }
    }
}
