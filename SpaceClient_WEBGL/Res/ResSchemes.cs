using Common;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using System;

namespace Nebula.Client.Res {
    public class ResSchemes {

        private Dictionary<Workshop, ResSchemeData> schemes;

        public void Load(string xml) {
            XDocument document = XDocument.Parse(xml);
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
