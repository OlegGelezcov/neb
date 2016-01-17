
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
    public class PetParameters {
        public PetUpgradeTable petUpgrades { get; private set; }
        public PetMasteryUpgradeTable masteryUpgrades { get; private set; }

        public void Load(string xml) {
#if UP
            UPXDocument document = new UPXDocument(xml);
#else
            XDocument document = XDocument.Parse(xml);
#endif

            petUpgrades = new PetUpgradeTable();
            petUpgrades.Load(document.Element("pets").Element("upgrades"));

            masteryUpgrades = new PetMasteryUpgradeTable();
            masteryUpgrades.Load(document.Element("pets").Element("mastery_upgrades"));
        }
    }
}
