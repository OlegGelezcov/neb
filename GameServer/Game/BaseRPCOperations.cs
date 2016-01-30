using Common;
using ServerClientCommon;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Game {
    public class BaseRPCOperations {
        protected Hashtable CreateResponse(RPCErrorCode code) {
            return new Hashtable {
                { (int)SPC.ReturnCode, (int)code }
            };
        }

        protected int SPCKEY(SPC code) {
            return (int)code;
        }
    }
}
