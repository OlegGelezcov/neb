using Common;
using Photon.SocketServer.Rpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Login.Operations {
    public class RegisterUserResponse {

        [DataMember(Code =(byte)ParameterCode.Login)]
        public string login { get; set; }

        [DataMember(Code =(byte)ParameterCode.GameRefId)]
        public string gameRef { get; set; }

        [DataMember(Code =(byte)ParameterCode.Status)]
        public int returnCode { get; set; }

    }
}
