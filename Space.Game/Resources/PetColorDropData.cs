using Common;
using System.Xml.Linq;

namespace Nebula.Resources {
    public class PetColorDropData : PetColorData {

        private float m_Prob;

        public PetColorDropData(XElement element) 
            : base(element) {
            m_Prob = element.GetFloat("prob");
        }

        public float prob {
            get {
                return m_Prob;
            }
        }
    }
}
