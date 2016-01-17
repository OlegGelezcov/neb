using Common;
using Nebula.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Nebula.Pets {
    public class PetDefaultModelTable : KeyValueTable<Race, string> {
        public PetDefaultModelTable(XElement element) {
            Load(element);
        }

        public override void Load(XElement element) {
            var dump = element.Elements("entry").Select(entryElement => {
                Race race = (Race)Enum.Parse(typeof(Race), entryElement.GetString("race"));
                string model = entryElement.GetString("model");
                this[race] = model;
                return race;
            }).ToList();
        }

    }
}
