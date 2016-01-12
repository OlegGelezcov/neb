using Common;
using System;
using System.Linq;
using System.Xml.Linq;

namespace Nebula.Resources {
    public class PetActiveSkillCountTable : KeyValueTable<PetColor, int> {

        public PetActiveSkillCountTable(XElement element) {
            Load(element);
        }

        public override void Load(XElement element) {
            var dump = element.Elements("color").Select(colorElement => {
                PetColor color = (PetColor)Enum.Parse(typeof(PetColor), colorElement.GetString("name"));
                int count = colorElement.GetInt("count");
                this[color] = count;
                return color;
            }).ToList();
        }
    }
}
