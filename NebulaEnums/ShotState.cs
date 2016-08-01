using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common {
    /// <summary>
    /// Describe current shot state
    /// </summary>
    public enum ShotState : byte {
        normal = 1,
        normalCritical = 2,
        missed = 3,
        blockedByDistance = 4,
        blockedBySkill = 5,
        blockedByObjectType = 6,
        
    }
}
