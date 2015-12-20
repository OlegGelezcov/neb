using Common;
#if UP
using Nebula.Client.UP;
#else
using System.Xml.Linq;
#endif

namespace Nebula.Client.Res {
    public class ResConsumableInfo {
        public string id { get; private set; }
        public string name { get; private set; }
        public string description { get; private set; }
#if UP
        public ResConsumableInfo(UPXElement element) {
            id = element.GetString("id");
            name = element.GetString("name");
            description = element.GetString("desc");
        }
#else
        public ResConsumableInfo(XElement element) {
            id = element.GetString("id");
            name = element.GetString("name");
            description = element.GetString("desc");
        }
#endif

    }
}
