using System.Collections.Concurrent;
using System.Linq;
using System.Xml.Linq;

namespace Nebula.Resources {
    public class ColorListCollection {

        public ConcurrentDictionary<string, ColorList> colorLists { get; private set; }

        public void Load(string file ) {
            XDocument document = XDocument.Load(file);
            colorLists = new ConcurrentDictionary<string, ColorList>();
            var dump = document.Element("color_lists").Elements("color_list").Select(colorListElement => {
                ColorList colorList = new ColorList(colorListElement);
                colorLists.TryAdd(colorList.id, colorList);
                return colorList;
            }).ToList();
        }

        public ColorList GetList(string id) {
            if(colorLists.ContainsKey(id)) {
                ColorList resultColorList;
                if(colorLists.TryGetValue(id, out resultColorList)) {
                    return resultColorList;
                }
            }
            return null;
        }

        public bool HasColorList(string id) {
            return colorLists.ContainsKey(id);
        }
    }
}
