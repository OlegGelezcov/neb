using Common;
#if UP
using Nebula.Client.UP;
#else
using System.Xml.Linq;
#endif

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
#if UP
        public HelpSubSection(UPXElement element) {
            id = element.GetString("id");
            titleId = element.GetString("title");
            contentId = element.GetString("content");
        }
#else
        public HelpSubSection(XElement element) {
            id = element.GetString("id");
            titleId = element.GetString("title");
            contentId = element.GetString("content");
        }
#endif
    }
}
