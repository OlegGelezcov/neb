using Nebula.Engine;

namespace Nebula.Game.Pets {
    public abstract class Condition {

        private NebulaObject m_Source;
        private PetObject m_Pet;

        public Condition(NebulaObject inSource) {
            m_Source = inSource;
            m_Pet = m_Source.GetComponent<PetObject>();
        }

        public abstract bool Check();
        public abstract void Renew();

        protected NebulaObject source {
            get {
                return m_Source;
            }
        }

        protected PetObject pet {
            get {
                return m_Pet;
            }
        }
    }
}
