using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Nebula.Client.Res {
    public class ResConsumableInfo {
        public string id { get; private set; }
        public string name { get; private set; }
        public string description { get; private set; }

        public ResConsumableInfo(XElement element) {
            id = element.GetString("id");
            name = element.GetString("name");
            description = element.GetString("desc");
        }

    }
}
