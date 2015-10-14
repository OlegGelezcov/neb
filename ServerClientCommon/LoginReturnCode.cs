using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerClientCommon {
    public enum LoginReturnCode : int {
        Ok = 0,
        UserWithSameLoginAlreadyExists = 1,
        InvaligLoginCharacter = 2,
        InvaligLoginLength = 3,
        UpdateGameRefOnClient = 4
    }
}
