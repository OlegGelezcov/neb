/*
using System.Collections.Generic;
using System.Linq;
#if UP
using Nebula.Client.UP;
#else
using System.Xml.Linq;
#endif

namespace Nebula.Client.Inaps {
    public class Subscriptions {

        public Dictionary<string, SubscriptionItem> subscriptions { get; private set; }

        public void Load(string xml) {
#if UP
            UPXDocument document = new UPXDocument(xml);
#else
            XDocument document = XDocument.Parse(xml);
#endif
            if (subscriptions == null) {
                subscriptions = new Dictionary<string, SubscriptionItem>();
            }
            subscriptions.Clear();

            var dump = document.Element("subscriptions").Elements("subscription").Select(sub => {
                SubscriptionItem item = new SubscriptionItem(sub);
                subscriptions.Add(item.id, item);
                return item;
            }).ToList();

        }

        public SubscriptionItem GetSubScription(string id) {
            if (subscriptions.ContainsKey(id)) {
                return subscriptions[id];
            }
            return null;
        }

        public List<SubscriptionItem> GetCountrySubscriptions(string country) {
            List<SubscriptionItem> result = new List<SubscriptionItem>();
            foreach (var pItem in subscriptions) {
                if (pItem.Value.country == country) {
                    result.Add(pItem.Value);
                }
            }
            result.Sort((first, second) => {
                return first.id.CompareTo(second.id);
            });
            return result;
        }
    }
}
*/