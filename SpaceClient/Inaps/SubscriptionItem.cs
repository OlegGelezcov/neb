using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Nebula.Client.Inaps {
    public class SubscriptionItem {
        public string id { get; private set; }
        public string country { get; private set; }
        public string description { get; private set; }
        public string discount { get; private set; }
        public string name { get; private set; }
        public string price { get; private set; }
        public string icon { get; private set; }

        public SubscriptionItem(XElement e) {
            id = e.GetString("id");
            name = e.GetString("name");
            description = e.GetString("description");
            price = e.GetString("price");
            country = e.GetString("country");
            discount = e.GetString("discount");
            icon = e.GetString("icon");
        }
    }
}
