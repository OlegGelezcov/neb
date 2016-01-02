
using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Resources {
    public abstract class PetParameter {
        public PetColor color { get; private set; }

        public PetParameter(PetColor color ) {
            this.color = color;
        }
    }
}
