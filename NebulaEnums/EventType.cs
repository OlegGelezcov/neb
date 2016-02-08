using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common {
    public enum EventType : int {
        GameObjectDeath = 1,
        None = 2,
        TriggerExit = 3,
        TriggerEnter = 4,
        TriggerStay = 5,
    }
}
