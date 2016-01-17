using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if UP
using Nebula.Client.UP;
#else
using System.Xml.Linq;
#endif

namespace Nebula.Client.Pets {
    public class PetMasteryUpgradeTable : KeyValueTable<int, PetMasteryUpgradeRequirement> {
        public override void Load(object elementObj) {
#if UP
            UPXElement element = elementObj as UPXElement;
#else
            XElement element = elementObj as XElement;
#endif
            var dump = element.Elements("upgrade").Select(ue => {
                PetMasteryUpgradeRequirement req = new PetMasteryUpgradeRequirement(ue);
                this[req.mastery] = req;
                return req;
            }).ToList();
        }
    }
}
