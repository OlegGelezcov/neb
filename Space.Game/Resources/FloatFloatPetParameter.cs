using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Resources {
    public class FloatFloatPetParameter : PetParameter  {

        public float first { get; private set; }
        public float second { get; private set; }

        public FloatFloatPetParameter(PetColor color, float first, float second ) : base(color) {
            this.first = first;
            this.second = second;
        }
    }
}
