using Common;
using ServerClientCommon;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelectCharacter.Notifications {
    public class Notification  : IInfoSource {

        public string uniqueID { get; set; } = string.Empty;
        public int respondAction { get; set; }
        public int sourceServiceType { get; set; }
        public string id { get; set; }
        public string text { get; set; }
        public Hashtable data { get; set; }
        public bool handled { get; set; }
        public int subType { get; set; }

        public Hashtable GetInfo() {
            return new Hashtable {
                { (int)SPC.Id, id },
                { (int)SPC.RespondAction, respondAction },
                { (int)SPC.SourceServiceType, sourceServiceType  },
                { (int)SPC.Text, text },
                { (int)SPC.Data, data },
                { (int)SPC.Handled, handled  },
                { (int)SPC.SubType, subType }
            };
        }
    }
}
