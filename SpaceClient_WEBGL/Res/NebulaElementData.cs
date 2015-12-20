using Common;
#if UP
using Nebula.Client.UP;
#else
using System.Xml.Linq;
#endif

namespace Nebula.Client.Res {
    public class NebulaElementData {
        public string id { get; private set; }
        public string name { get; private set; }
        public string description { get; private set; }

        public string iconPath { get; private set; }

#if UP
        public NebulaElementData(UPXElement element) {
            id = element.GetString("id");
            name = element.GetString("name");
            description = element.GetString("description");
            iconPath = element.GetString("icon");
        }
#else
        public NebulaElementData(XElement element) {
            id = element.GetString("id");
            name = element.GetString("name");
            description = element.GetString("description");
            iconPath = element.GetString("icon");
        }
#endif
    }
}
