using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Nebula.Resources {
    public class DifficultyTable : KeyValueTable<Difficulty, float> {

        public void LoadFile(string fileName ) {
            XDocument document = XDocument.Load(fileName);
            Load(document.Element("difficulty"));
        }

        public override void Load(XElement element) {
            var dump = element.Elements("mult").Select(multElement => {
                Difficulty difficulty = (Difficulty)System.Enum.Parse(typeof(Difficulty), multElement.GetString("name"));
                float val = multElement.GetFloat("value");
                this[difficulty] = val;
                return difficulty;
            }).ToList();
        }
    }
}
