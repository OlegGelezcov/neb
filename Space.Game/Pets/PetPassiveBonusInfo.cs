using Common;
using System;
using System.Xml.Linq;

namespace Nebula.Pets {
    public class PetPassiveBonusInfo {
        private int m_Id;
        private BonusType m_BonusType;
        private float m_Value;

        public PetPassiveBonusInfo(XElement element) {
            m_Id = element.GetInt("id");
            m_BonusType = (BonusType)Enum.Parse(typeof(BonusType), element.GetString("type"));
            m_Value = element.GetFloat("value");
        }

        public int id {
            get {
                return m_Id;
            }
        }

        public BonusType bonusType {
            get {
                return m_BonusType;
            }
        }

        public float value {
            get {
                return m_Value;
            }
        }
    }
}
