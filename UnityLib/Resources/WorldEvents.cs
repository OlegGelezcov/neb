namespace Nebula.Resources {
    using System.Collections.Generic;


    public class WorldEventData {
        public string worldId;
        public string id;
        public string name;
        public string text_start;
        public string text_end;
        public string text_hint;
        public string voice_start;
        public string voice_end;
        public string owner;
    }

    public class WorldEvents {
        public string worldId;
        public List<WorldEventData> events;

        public WorldEventData GetEvent(string id) {
            foreach (var e in this.events) {
                if (e.id == id) {
                    return e;
                }
            }
            return null;
        }
    }

    public class WorldEventsCache {

        public bool Loaded { get; private set; }

        private Dictionary<string, WorldEvents> _worlds = new Dictionary<string, WorldEvents>();

        public WorldEvents GetWorld(string id) {
            foreach (var kv in this._worlds) {
                if (kv.Value.worldId == id) {
                    return kv.Value;
                }
            }
            return null;
        }

        public void Load() {
            sXDocument document = new sXDocument();
            document.Parse("Texts/world_events");
            sXElement root = document.GetElement("world_events");
            this._worlds = new Dictionary<string, WorldEvents>();
            foreach (var elem in root.GetElements("world")) {
                var ew = LoadWorldEvents(elem);
                this._worlds.Add(ew.worldId, ew);
            }
            Loaded = true;
        }

        private static WorldEvents LoadWorldEvents(sXElement element) {
            string id = element.GetString("id");
            List<WorldEventData> events = new List<WorldEventData>();
            foreach (var elem in element.GetElements("event")) {
                var e = LoadEvent(elem, id);
                events.Add(e);
            }
            return new WorldEvents { worldId = id, events = events };
        }

        private static WorldEventData LoadEvent(sXElement element, string worldId) {
            string id = element.GetString("id");
            string name = element.GetString("name");
            sXElement data = element.GetElement("data");
            string text_start = data.GetString("text_start");
            string text_end = data.GetString("text_end");
            string text_hint = data.GetString("text_hint");
            string voice_start = data.GetString("voice_start");
            string voice_end = data.GetString("voice_end");
            string owner = data.GetString("owner");
            return new WorldEventData { id = id, name = name, owner = owner, text_start = text_start, text_end = text_end, text_hint = text_hint, voice_start = voice_start, voice_end = voice_end, worldId = worldId };
        }
    }
}
