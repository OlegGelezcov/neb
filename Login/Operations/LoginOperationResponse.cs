using Common;
using Photon.SocketServer.Rpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Login.Operations {
    public class LoginOperationResponse {

        [DataMember(Code =(byte)ParameterCode.GameRefId)]
        public string GameRefId { get; set; }

        [DataMember(Code =(byte)ParameterCode.Login)]
        public string Login { get; set; }

        [DataMember(Code = (byte)ParameterCode.FacebookId)]
        public string facebookId { get; set; }

        [DataMember(Code = (byte)ParameterCode.VkontakteId)]
        public string vkontakteId { get; set; }

        [DataMember(Code = (byte)ParameterCode.Method)]
        public byte method { get; set; }

        [DataMember(Code =(byte)ParameterCode.Status)]
        public int returnCode { get; set; }

    }
}
