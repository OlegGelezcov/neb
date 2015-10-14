using Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Nebula.Resources {
    public class ResSchemeCraftingMaterials {

        public ConcurrentBag<SchemeCraftingMaterial> materials { get; private set; }

        public void Load(string basePath) {
            string fullPath = Path.Combine(basePath, "Data/Drop/scheme_crafting_materials.xml");
            XDocument document = XDocument.Load(fullPath);
            materials = new ConcurrentBag<SchemeCraftingMaterial>();
            var dump = document.Element("crafting").Elements("sceme").Select(e => {
                materials.Add(new SchemeCraftingMaterial(e));
                return e;
            }).ToList();
        }

        public bool Contains(int level, ShipModelSlotType slotType) {
            foreach(var m in materials) {
                if(m.level == level && m.slotType == slotType ) {
                    return true;
                }
            }
            return false;
        }

        public Dictionary<string, int> GetCraftingMaterials(int level, ShipModelSlotType slotType, ObjectColor objColor) {
            Dictionary<string, int> result = new Dictionary<string, int>();
            foreach(var m in materials) {
                if(m.level == level && m.slotType == slotType) {
                    foreach(var p in m.craftingMaterials) {
                        result.Add(p.Key, p.Value + AddForColor(objColor));
                    }
                    return result;
                }
            }
            return result;
        }

        private int AddForColor(ObjectColor color) {
            switch(color) {
                case ObjectColor.white: return 0;
                case ObjectColor.blue: return 1;
                case ObjectColor.yellow: return 2;
                case ObjectColor.green: return 3;
                case ObjectColor.orange: return 4;
                default: return 0;
            }
        }
    }
}
