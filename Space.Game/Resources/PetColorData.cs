using Common;
using System;
using System.Xml.Linq;

namespace Nebula.Resources {
    public class PetColorData {
        private PetColor m_Color;
        private float m_Mult;

        public PetColorData(XElement element) {
            m_Color = (PetColor)Enum.Parse(typeof(PetColor), element.GetString("name"));
            m_Mult = element.GetFloat("mult");
        }

        public PetColor color {
            get {
                return m_Color;
            }
        }

        public float mult {
            get {
                return m_Mult;
            }
        }
    }
}
