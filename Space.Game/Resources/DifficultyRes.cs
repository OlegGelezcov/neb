using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Nebula.Resources {
    public class DifficultyRes {
        public DifficultyTable weapon { get; private set; }
        public DifficultyTable module { get; private set; }

        public DifficultyRes() {
            weapon = new DifficultyTable();
            module = new DifficultyTable();
        }

        public void Load(string fileName) {
            XDocument document = XDocument.Load(fileName);
            weapon.Load(document.Element("difficulty").Element("weapon"));
            module.Load(document.Element("difficulty").Element("weapon"));
        }
    }
}
