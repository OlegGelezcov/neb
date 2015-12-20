using System.Collections.Generic;
using System.Linq;
#if UP
using Nebula.Client.UP;
#else
using System.Xml.Linq;
#endif

namespace Nebula.Client.Help {
    public class Tips {

        public List<TipInfo> tips { get; private set; }

        public Tips() {
            tips = new List<TipInfo>();
        }

        public void Load(string xml) {
#if UP
            UPXDocument document = new UPXDocument(xml);
            tips = document.Element("tips").Elements("tip").Select(e => {
                return new TipInfo(e);
            }).ToList();
#else
            XDocument document = XDocument.Parse(xml);
            tips = document.Element("tips").Elements("tip").Select(e => {
                return new TipInfo(e);
            }).ToList();
#endif
        }

        public TipInfo GetTip(string id) {
            foreach (var tip in tips) {
                if (tip.id == id) {
                    return tip;
                }
            }
            return null;
        }

        public TipInfo GetTipByEvent(string eventName) {
            foreach (var tip in tips) {
                if (tip.eventName == eventName) {
                    return tip;
                }
            }
            return null;
        }
    }
}
