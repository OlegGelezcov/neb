using Common;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Nebula.Client.Help {
    public class HelpSection {

        public string id { get; private set; }
        public string titleId { get; private set; }
        public string contentId { get; private set; }
        public List<HelpSubSection> subSections { get; private set; }

        public HelpSection(string id, string title, string content, List<HelpSubSection> subSections) {
            this.id = id;
            this.titleId = title;
            this.contentId = content;
            this.subSections = subSections;
        }

        public HelpSection(XElement element) {
            id = element.GetString("id");
            titleId = element.GetString("title");
            contentId = element.GetString("content");

            subSections = element.Elements("subsection").Select(e => {
                return new HelpSubSection(e);
            }).ToList();
        }

        public bool HasSubSection(string subID) {
            foreach (var ss in subSections) {
                if (ss.id == subID) {
                    return true;
                }
            }
            return false;
        }

        public HelpSubSection GetSubSection(string subID) {
            foreach (var ss in subSections) {
                if (ss.id == subID) {
                    return ss;
                }
            }
            return null;
        }
    }
}
