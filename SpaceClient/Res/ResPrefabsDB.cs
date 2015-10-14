using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Common;

namespace Nebula.Client.Res {
    public class ResPrefabsDB {

        public Dictionary<string, string> prefabs { get; private set; }

        public void Load(string xml) {
            XDocument document = XDocument.Parse(xml);
            prefabs = document.Element("prefabs").Elements("prefab").Select(prefabElement => {
                string id = prefabElement.GetString("id");
                string path = prefabElement.GetString("path");
                return new { id = id, path = path };
            }).ToDictionary(obj => obj.id, obj => obj.path);
        }

        public bool TryGetPath(string prefabId, out string path) {
            path = string.Empty;
            if(prefabs.ContainsKey(prefabId)) {
                path = prefabs[prefabId];
                return true;
            }
            return false;
        }
    }
}
