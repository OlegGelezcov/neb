using Common;
using System.Collections.Concurrent;
using System.Linq;
using System.Xml.Linq;

namespace Nebula.Resources {
    public class DropListCollection {

        public ConcurrentDictionary<string, Nebula.Drop.DropList> dropLists { get; private set; }

        public void Load(string file) {
            dropLists = new ConcurrentDictionary<string, Nebula.Drop.DropList>();
            XDocument document = XDocument.Load(file);

            var dump = document.Element("drop_lists").Elements("drop_list").Select(dropListElement => {
                Nebula.Drop.DropList dropList = new Nebula.Drop.DropList(dropListElement);
                dropLists.TryAdd(dropList.id, dropList);
                return dropList;
            }).ToList();
        }

        public Nebula.Drop.DropList GetList(string id ) {
            Nebula.Drop.DropList resultDropList = null;
            if(dropLists.TryGetValue(id, out resultDropList)) {
                return resultDropList;
            }
            return null;
        }
    }
}
