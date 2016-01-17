using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Common {
    public class IDCountPair {
        public string ID { get; set; }
        public int count { get; set; }

        public IDCountPair() {
            ID = string.Empty;
            count = 0;
        }

        public IDCountPair(XElement element) {
            ID = element.GetString("id");
            count = element.GetInt("count");
        }
    }
}
