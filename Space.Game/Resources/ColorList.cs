using Common;
using GameMath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Nebula.Resources {
    public class ColorList {

        public class ColorEntry {
            public ObjectColor color { get; private set; }
            public float prob { get; private set; }

            public ColorEntry(ObjectColor c, float p) {
                color = c;
                prob = p;
            }

            public ColorEntry(XElement element) {
                color = (ObjectColor)Enum.Parse(typeof(ObjectColor), element.GetString("name"));
                prob = element.GetFloat("prob");
            }
        }


        public string id { get; private set; }
        public List<ColorEntry> entries { get; private set; }

        public ColorList(XElement element) {
            id = element.GetString("id");
            entries = element.Elements("item").Select(itemElement => {
                return new ColorEntry(itemElement);
            }).ToList();
        }

        public ObjectColor Roll() {
            foreach(var entry in entries ) {
                if(Rand.Float01() < entry.prob ) {
                    return entry.color;
                }
            }
            return entries[entries.Count - 1].color;
        }
    }
}
