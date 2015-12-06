using Common;
using System.Xml.Linq;

namespace Nebula.Client.Res {
    public class NebulaElementData {
        public string id { get; private set; }
        public string name { get; private set; }
        public string description { get; private set; }

        public string iconPath { get; private set; }


        public NebulaElementData(XElement element) {
            id = element.GetString("id");
            name = element.GetString("name");
            description = element.GetString("description");
            iconPath = element.GetString("icon");
        }
    }
}
