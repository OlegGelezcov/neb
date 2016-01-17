using Common;
using Nebula.Resources;
using System.Linq;
using System.Xml.Linq;

namespace Nebula.Pets {
    public class PetUpgradeTable : KeyValueTable<PetColor, PetUpgradeRequirement> {
        public override void Load(XElement element) {
            var dump = element.Elements("upgrade").Select(ue => {
                PetUpgradeRequirement req = new PetUpgradeRequirement(ue);
                this[req.color] = req;
                return req;
            }).ToList();
        }

        public PetUpgradeTable(XElement element) {
            Load(element);
        }
    }
}
