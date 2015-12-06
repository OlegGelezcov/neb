using Common;
using System.Xml.Linq;

namespace Nebula.Client.Help {
    public class TipInfo {
        public string id { get; private set; }
        public string textId { get; private set; }
        public TipType type { get; private set; }
        public string image { get; private set; }
        public string eventName { get; private set; }

        public TipInfo(XElement element) {
            id = element.GetString("id");
            textId = element.GetString("text");
            type = (TipType)System.Enum.Parse(typeof(TipType), element.GetString("type"));
            image = element.GetString("image");
            eventName = element.GetString("event_name");
        }
    }

    public enum TipType {
        one,
        loop
    }
}
