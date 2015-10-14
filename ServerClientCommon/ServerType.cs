using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerClientCommon {
    public enum ServerType : byte {
        login           = 0,
        character       = 1,
        game            = 2,
        chat            = 3,
        master = 4
    }
}
