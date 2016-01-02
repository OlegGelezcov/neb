
using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Resources {
    public class IntFloatPetParameter : PetParameter {
        public int first { get; private set; }
        public float second { get; private set; }

        public IntFloatPetParameter(PetColor color, int first, float second)
            : base(color) {
            this.first = first;
            this.second = second;
        } 
    }
}
