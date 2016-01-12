using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Game.Pets {
    public abstract class PetBaseState {

        private PetObject m_Pet;

        public PetBaseState(PetObject inPet ) {
            m_Pet = inPet;
        }

        public abstract void Update(float deltaTime);
        public abstract PetState name { get; }

        protected PetObject pet {
            get {
                return m_Pet;
            }
        }
    }
}
