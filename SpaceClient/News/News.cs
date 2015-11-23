using Common;
using System.Collections;
using System.Collections.Generic;

namespace Nebula.Client.News {
    public class News : IInfoParser{

        
        public Dictionary<string, PostEntry> entries { get; private set; }

        public News() {
            entries = new Dictionary<string, PostEntry>();
        }

        public News(Hashtable info) {
            entries = new Dictionary<string, PostEntry>();
            ParseInfo(info);
        }

        public void ParseInfo(Hashtable info) {
            if(info != null ) {
                entries.Clear();
                foreach(DictionaryEntry pEntry in info) {
                    string id = (string)pEntry.Key;
                    Hashtable entryInfo = pEntry.Value as Hashtable;
                    if(entryInfo != null ) {
                        entries.Add(id, new PostEntry(entryInfo));
                    }
                }
            }
        }


        public void FromJson(string json) {
            var newsList = MiniJSON.Json.Deserialize(json) as List<object>;
            if(entries == null ) {
                entries = new Dictionary<string, PostEntry>();
            }
            entries.Clear();

            if(newsList != null ) {
                foreach(object postObject in newsList ) {
                    Dictionary<string, object> postDictionary = postObject as Dictionary<string, object>;
                    if(postDictionary != null ) {
                        string postId = postDictionary["post_id"].ToString();
                        int time = int.Parse(postDictionary["time"].ToString());
                        string message = postDictionary["message"].ToString();
                        string lang = postDictionary["lang"].ToString();
                        string postUrl = postDictionary["post_url"].ToString();
                        string imageUrl = postDictionary["image_url"].ToString();
                        entries.Add(postId, new PostEntry(postId, time, message, lang, postUrl, imageUrl));

                    }
                }
            }
        }
    }
}
