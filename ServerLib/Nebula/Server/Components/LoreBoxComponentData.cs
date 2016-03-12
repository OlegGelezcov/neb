using Common;
using System.Xml.Linq;

namespace Nebula.Server.Components {
    public class LoreBoxComponentData : ComponentData {
        public string loreRecordId { get; private set; }

        public LoreBoxComponentData(XElement element) {
            loreRecordId = element.GetString("record_id");
        }

        public override ComponentID componentID {
            get {
                return ComponentID.LoreBox;
            }
        }
    }
}
