using Common;
using System.Xml.Linq;

namespace Nebula.Server.Components {
    public class SkillsComponentData : ComponentData {
        public SkillsComponentData(XElement e) {

        }

        public SkillsComponentData() {

        }
        public override ComponentID componentID {
            get {
                return ComponentID.Skills;
            }
        }
    }
}
