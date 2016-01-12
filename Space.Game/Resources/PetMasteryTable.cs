using Common;
using System.Linq;
using System.Xml.Linq;

namespace Nebula.Resources {
    public class PetMasteryTable : KeyValueTable<int, float> {
        public PetMasteryTable(XElement element) {
            Load(element);
        }

        public override void Load(XElement element) {
            var dump = element.Elements("tier").Select(tierElement => {
                int tier = tierElement.GetInt("id");
                float prob = tierElement.GetFloat("prob");
                this[tier] = prob;
                return tier;
            }).ToList();
        }
    }
}
