using Common;
using System.Collections.Concurrent;
using System.Linq;
using System.Xml.Linq;

namespace Nebula.Resources {
    public class PetColorDropDataResource {
        private ConcurrentDictionary<PetColor, PetColorDropData> m_DropColors;

        public PetColorDropDataResource(XElement element) {
            m_DropColors = new ConcurrentDictionary<PetColor, PetColorDropData>();
            var dump = element.Elements("color").Select(colorElement => {
                PetColorDropData colorData = new PetColorDropData(colorElement);
                m_DropColors.TryAdd(colorData.color, colorData);
                return colorData.color;
            }).ToList();
        }

        public PetColorDropData GetColor(PetColor color ) {
            PetColorDropData colorData;
            if(m_DropColors.TryGetValue(color, out colorData)) {
                return colorData;
            }
            return null;
        }
    }
}
