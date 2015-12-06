using Common;
using System.Xml.Linq;

namespace Nebula.Client.Help {
    public class HelpSubSection {
        public string id { get; private set; }
        public string titleId { get; private set; }
        public string contentId { get; private set; }

        public HelpSubSection(string id, string title, string content) {
            this.id = id;
            this.titleId = title;
            this.contentId = content;
        }

        public HelpSubSection(XElement element) {
            id = element.GetString("id");
            titleId = element.GetString("title");
            contentId = element.GetString("content");
        }
    }
}
