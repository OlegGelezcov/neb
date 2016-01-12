using Common;
using Nebula.Pets;
using Space.Game;
using System;
using System.Xml.Linq;

namespace Nebula.Resources {
    public class MainPetParameterResource : IPetParamResource {


        public class DataValue {
            private float m_Value;
            private float m_RandMin;
            private float m_RandMax;

            public DataValue(XElement element) {
                m_Value = element.GetFloat("value");
                m_RandMin = element.GetFloat("rnd_min");
                m_RandMax = element.GetFloat("rnd_max");
            }

            public float value {
                get {
                    return m_Value;
                }
            }

            public float randMin {
                get {
                    return m_RandMin;
                }
            }

            public float randMax {
                get {
                    return m_RandMax;
                }
            }
        }


        private DataValue m_BaseValue;
        private DataValue m_ColorValue;
        private DataValue m_LevelValue;
        private float m_RandMin;
        private float m_RandMax;
        private PetColorDataResource m_ColorResource;

        public MainPetParameterResource(XElement element) {
            var baseElement = element.Element("base");
            var colorElement = element.Element("color");
            var levelElement = element.Element("level");
            m_BaseValue = new DataValue(baseElement);
            m_ColorValue = new DataValue(colorElement);
            m_LevelValue = new DataValue(levelElement);
            m_RandMin = element.GetFloat("rnd_min");
            m_RandMax = element.GetFloat("rnd_max");

            var colorsElement = element.Element("colors");
            m_ColorResource = new PetColorDataResource(colorsElement);
        }

        #region Properties
        private DataValue baseValue {
            get {
                return m_BaseValue;
            }
        }
        private DataValue colorValue {
            get {
                return m_ColorValue;
            }
        }
        private DataValue levelValue {
            get {
                return m_LevelValue;
            }
        }
        private float randMin {
            get {
                return m_RandMin;
            }
        }
        private float randMax {
            get {
                return m_RandMax;
            }
        } 
        private PetColorDataResource colorResource {
            get {
                return m_ColorResource;
            }
        }
        #endregion

        #region Interface IPetParamResource
        public float BaseValue() {
            return baseValue.value;
        }

        public float ColorMult(PetInfo pet) {
            var color = colorResource.GetColor(pet.color);
            if(color != null ) {
                return color.mult;
            }
            return 0f;
        }

        public float ColorValue() {
            return colorValue.value;
        }


        public float LevelValue() {
            return levelValue.value;
        }

        public float BaseMin() {
            return baseValue.randMin;
        }

        public float BaseMax() {
            return baseValue.randMax;
        }

        public float ColorMin() {
            return colorValue.randMin;
        }

        public float ColorMax() {
            return colorValue.randMax;
        }

        public float LevelMin() {
            return levelValue.randMin;
        }

        public float LevelMax() {
            return levelValue.randMax;
        }
        #endregion
    }
}
