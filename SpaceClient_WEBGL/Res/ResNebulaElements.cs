
using System.Collections.Generic;
using System.Linq;
#if UP
using Nebula.Client.UP;
#else
using System.Xml.Linq;
#endif

namespace Nebula.Client.Res {
    public class ResNebulaElements {

        public Dictionary<string, NebulaElementData> nebulaElements { get; private set; }

        public void Load(string xml) {
#if UP
            UPXDocument document = new UPXDocument(xml);
#else
            XDocument document = XDocument.Parse(xml);
#endif
            nebulaElements = new Dictionary<string, NebulaElementData>();

            var dumpList = document.Element("nebula_elements").Elements("element").Select(element => {
                NebulaElementData data = new NebulaElementData(element);
                nebulaElements.Add(data.id, data);
                return data;
            }).ToList(); 
        }

        public bool TryGetData(string id, out NebulaElementData data) {
            return nebulaElements.TryGetValue(id, out data);
        }
    }
}
