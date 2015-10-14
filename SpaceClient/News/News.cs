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


    }
}
