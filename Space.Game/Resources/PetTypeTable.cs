using Common;
using GameMath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Nebula.Resources {
    public class PetTypeTable : KeyValueTable<string, Race> {
        public PetTypeTable(XElement element) {
            Load(element);
        }

        public override void Load(XElement element) {
            var dump = element.Elements("type").Select(typeElement => {
                string id = typeElement.GetString("id");
                Race race = (Race)Enum.Parse(typeof(Race), typeElement.GetString("race"));
                this[id] = race;
                return id;
            }).ToList();
        }

        public List<string> GetTypes(Race race) {
            List<string> ids = new List<string>();
            foreach(var kvp in dict ) {
                if(kvp.Value == race ) {
                    ids.Add(kvp.Key);
                }
            }
            return ids;
        }

        public string GetRandomType() {
            List<string> ids = new List<string>(dict.Keys);
            return ids[Rand.Int(0, ids.Count - 1)];
        }

        public string GetRandomType(Race race) {
            List<string> ids = new List<string>();
            foreach(var pair in dict) {
                if(pair.Value == race) {
                    ids.Add(pair.Key);
                }
            }
            if(ids.Count == 0 ) {
                return string.Empty;
            } else {
                return ids[Rand.Int(0, ids.Count - 1)];
            }
        }

        public bool HasType(string skin) {
            return dict.ContainsKey(skin);
        }
    }
}
