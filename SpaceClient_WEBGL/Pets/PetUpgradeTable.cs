using Common;
using System.Linq;

#if UP
using Nebula.Client.UP;
#else
using System.Xml.Linq;
#endif

namespace Nebula.Client.Pets {
    public class PetUpgradeTable : KeyValueTable<PetColor, PetUpgradeRequirement> {
        public override void Load(object elementObj) {
#if UP
            var element = elementObj as UPXElement;
#else
            var element = elementObj as XElement;
#endif
            var dump = element.Elements("upgrade").Select(ue => {
                PetUpgradeRequirement req = new PetUpgradeRequirement(ue);
                this[req.color] = req;
                return req;
            }).ToList();
        }
    }
}
