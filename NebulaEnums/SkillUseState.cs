using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common {
    public enum SkillUseState : byte {
        normal = 0,
        invalidTarget = 1,
        tooFar = 2,
    }
}
