using Common;
using System.Collections.Concurrent;
using System.Linq;
using System.Xml.Linq;

namespace Nebula.Resources {
    public class PetColorDataResource {
        private ConcurrentDictionary<PetColor, PetColorData> m_PetColors;

        public PetColorDataResource(XElement element ) {
            m_PetColors = new ConcurrentDictionary<PetColor, PetColorData>();
            var dump = element.Elements("color").Select(colorElement => {
                PetColorData data = new PetColorData(colorElement);
                m_PetColors.TryAdd(data.color, data);
                return data.color;
            }).ToList();
        }

        public PetColorData GetColor(PetColor name) {
            PetColorData data;
            if(m_PetColors.TryGetValue(name, out data)) {
                return data;
            }
            return null;
        }
    }
}
