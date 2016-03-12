using System.Collections.Generic;
using System.Linq;

namespace Nebula.Client.Lore {
    public class LoreStoryCollection {
        public Dictionary<string, LoreStoryData> stories { get; private set; }

        public void Load(string xml) {
            stories = new Dictionary<string, LoreStoryData>();
            UniXmlDocument document = new UniXmlDocument(xml);
            var dump = document.document.Element("stories").Elements("story").Select(se => {
                LoreStoryData story = new LoreStoryData(new UniXMLElement(se));
                stories.Add(story.name, story);
                return story;
            }).ToList();
        }

        public LoreRecordData GetRecord(string id, out LoreStoryData story) {
            story = null;

            foreach(var pStory in stories ) {
                var rec = pStory.Value.GetRecord(id);
                if(rec != null ) {
                    story = pStory.Value;
                    return rec;
                }
            }
            return null;
        }

        public LoreStoryData GetStory(string name) {
            if(stories.ContainsKey(name)) {
                return stories[name];
            }
            return null;
        }
    }
}
