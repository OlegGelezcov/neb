using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NebulaCommon {
    public enum ServerType : byte {
        Master = 0,
        Login = 1,
        SelectCharacter = 2,
        Game = 3
    }
}
