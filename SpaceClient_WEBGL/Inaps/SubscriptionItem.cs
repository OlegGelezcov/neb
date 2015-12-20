/*
using Common;
#if UP
using Nebula.Client.UP;
#else
using System.Xml.Linq;
#endif

namespace Nebula.Client.Inaps {
    public class SubscriptionItem {
        public string id { get; private set; }
        public string country { get; private set; }
        public string description { get; private set; }
        public string discount { get; private set; }
        public string name { get; private set; }
        public string price { get; private set; }
        public string icon { get; private set; }
#if UP
        public SubscriptionItem(UPXElement e) {
            id = e.GetString("id");
            name = e.GetString("name");
            description = e.GetString("description");
            price = e.GetString("price");
            country = e.GetString("country");
            discount = e.GetString("discount");
            icon = e.GetString("icon");
        }
#else
        public SubscriptionItem(XElement e) {
            id = e.GetString("id");
            name = e.GetString("name");
            description = e.GetString("description");
            price = e.GetString("price");
            country = e.GetString("country");
            discount = e.GetString("discount");
            icon = e.GetString("icon");
        }
#endif
    }
}
*/