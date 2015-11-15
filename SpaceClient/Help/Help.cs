using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Nebula.Client.Help {
    public class Help {
        public List<HelpSection> sections { get; private set; }

        public Help() {
            sections = new List<HelpSection>();
        }

        public void Load(string xml ) {
            sections.Clear();
            XDocument document = XDocument.Parse(xml);
            sections = document.Element("help").Elements("section").Select(e => {
                return new HelpSection(e);
            }).ToList();
        }

        public bool HasSection(string id) {
            foreach(var section in sections ) {
                if(section.id == id ) {
                    return true;
                }
            }
            return false;
        }

        public HelpSection GetSection(string id ) {
            foreach(var section in sections ) {
                if(section.id == id ) {
                    return section;
                }
            }
            return null;
        }
    }
}
