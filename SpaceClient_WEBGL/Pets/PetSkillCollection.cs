
using System.Linq;

#if UP
using Nebula.Client.UP;
#else
using System.Xml.Linq;
#endif

namespace Nebula.Client.Pets {
    public class PetSkillCollection : KeyValueTable<int, PetSkillData> {
        public override void Load(object elementObj) {
#if UP
            var element = elementObj as UPXElement;
#else
            var element = elementObj as XElement;
#endif

            var dump = element.Elements("skill").Select(skillElement => {
                PetSkillData data = new PetSkillData(skillElement);
                this[data.id] = data;
                return data;
            }).ToList();
        }

        public void LoadText(string xml) {
#if UP
            UPXDocument document = new UPXDocument(xml);
#else
            XDocument document = XDocument.Parse(xml);
#endif

            Load(document.Element("skills"));
        }


    }
}
