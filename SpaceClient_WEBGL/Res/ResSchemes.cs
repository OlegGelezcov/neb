using Common;
using System.Collections.Generic;
using System.Linq;
using System;
#if UP
using Nebula.Client.UP;
#else
using System.Xml.Linq;
#endif

namespace Nebula.Client.Res {
    public class ResSchemes {

        private Dictionary<Workshop, ResSchemeData> schemes;

        public void Load(string xml) {
#if UP
            UPXDocument document = new UPXDocument(xml);
#else
            XDocument document = XDocument.Parse(xml);
#endif
            schemes = document.Element("schemes").Elements("scheme").Select(e => {
                Workshop w = (Workshop)Enum.Parse(typeof(Workshop), e.GetString("workshop"));
                string icon = e.GetString("icon");
                return new ResSchemeData(w, icon);
            }).ToDictionary(s => s.Workshop, s => s);
        }


        public bool TryGetScheme(Workshop workshop, out ResSchemeData scheme) {
            return this.schemes.TryGetValue(workshop, out scheme);
        }
    }
}
