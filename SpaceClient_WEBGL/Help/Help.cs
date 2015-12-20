using System.Collections.Generic;
using System.Linq;
#if UP
using Nebula.Client.UP;
#else
using System.Xml.Linq;
#endif

namespace Nebula.Client.Help {
    public class Help {
        public List<HelpSection> sections { get; private set; }

        public Help() {
            sections = new List<HelpSection>();
        }

        public void Load(string xml) {
            sections.Clear();
#if UP
            UPXDocument document = new UPXDocument(xml);
            sections = document.Element("help").Elements("section").Select(e => {
                return new HelpSection(e);
            }).ToList();
#else
            XDocument document = XDocument.Parse(xml);
            sections = document.Element("help").Elements("section").Select(e => {
                return new HelpSection(e);
            }).ToList();
#endif
        }

        public bool HasSection(string id) {
            foreach (var section in sections) {
                if (section.id == id) {
                    return true;
                }
            }
            return false;
        }

        public HelpSection GetSection(string id) {
            foreach (var section in sections) {
                if (section.id == id) {
                    return section;
                }
            }
            return null;
        }
    }
}
