using Common;
#if UP
using Nebula.Client.UP;
#else
using System.Xml.Linq;
#endif

namespace Nebula.Server.Components {
    public class SkillsComponentData : ComponentData {

#if UP
        public SkillsComponentData(UPXElement e) {

        }
#else
        public SkillsComponentData(XElement e) {

        }
#endif
        public SkillsComponentData() {

        }
        public override ComponentID componentID {
            get {
                return ComponentID.Skills;
            }
        }
    }
}
