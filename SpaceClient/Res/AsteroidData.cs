using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Common;

namespace Nebula.Client.Res {
    public class AsteroidData {
        public string ID { get; private set; }
        public string nameID { get; private set; }
        public int quality { get; private set; }

        public AsteroidData(XElement e) {
            ID = e.GetString("id");
            nameID = e.GetString("name");
            quality = e.GetInt("quality");
        }
    }
}
