
using System.Collections.Generic;
using System.Linq;
using Common;
#if UP
using Nebula.Client.UP;
#else
using System.Xml.Linq;
#endif

namespace Nebula.Client.Res {
    public class ResGameEvents {
        public Dictionary<string, Dictionary<string, ResGameEventData>> eventCaches { get; private set; }

        public Dictionary<string, Dictionary<string, ResGameEventData>> LoadFile(string xml) {
#if UP
            UPXDocument document = new UPXDocument(xml);
#else
            XDocument document = XDocument.Parse(xml);
#endif

            return document.Element("zones").Elements("zone").Select(zoneElement => {
                string zoneId = zoneElement.GetString("id");
                Dictionary<string, ResGameEventData> zoneGameEvents = zoneElement.Element("events").Elements("event").Select(eventElement => {
                    string eventId = eventElement.GetString("id");
                    float cooldown = eventElement.GetFloat("cooldown");
                    float radius = eventElement.GetFloat("radius");
                    string description = eventElement.GetString("description");
                    float[] position = eventElement.GetFloatArray("position");
                    return new ResGameEventData {
                        cooldown = cooldown,
                        descriptionId = description,
                        id = eventId,
                        position = position,
                        radius = radius
                    };
                }).ToDictionary(data => data.id, data => data);

                return new { ZoneId = zoneId, Events = zoneGameEvents };
            }).ToDictionary(data => data.ZoneId, data => data.Events);
        }

        public ResGameEvents() {
            eventCaches = new Dictionary<string, Dictionary<string, ResGameEventData>>();
        }

        public void AddEvents(Dictionary<string, Dictionary<string, ResGameEventData>> newEvents) {
            foreach(var pair in newEvents) {
                if(eventCaches.ContainsKey(pair.Key)) {
                    foreach(var pair2 in pair.Value) {
                        eventCaches[pair.Key].Add(pair2.Key, pair2.Value);
                    }
                } else {
                    eventCaches.Add(pair.Key, pair.Value);
                }
            }
        }

        public bool TryGetEvents(string worldId, out Dictionary<string, ResGameEventData> filteredEvents) {
            return eventCaches.TryGetValue(worldId, out filteredEvents);
        }

        public bool TryGetEvent(string worldId, string eventId, out ResGameEventData eventData) {
            eventData = null;
            Dictionary<string, ResGameEventData> filteredEvents = null;
            if (TryGetEvents(worldId, out filteredEvents)) {
                return filteredEvents.TryGetValue(eventId, out eventData);
            }
            return false;
        }
    }
}
