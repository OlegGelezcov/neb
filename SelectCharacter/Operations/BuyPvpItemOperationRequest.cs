using Common;
using Photon.SocketServer;
using Photon.SocketServer.Rpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelectCharacter.Operations {
    public class BuyPvpItemOperationRequest : Operation {
        public BuyPvpItemOperationRequest(IRpcProtocol protocol, OperationRequest request) 
            : base(protocol, request ) { }

        [DataMember(Code =(byte)ParameterCode.Login)]
        public string login { get; set; }

        [DataMember(Code =(byte)ParameterCode.GameRefId)]
        public string gameRef { get; set; }

        [DataMember(Code =(byte)ParameterCode.CharacterId)]
        public string character { get; set; }

        [DataMember(Code =(byte)ParameterCode.Race)]
        public byte race { get; set; }

        [DataMember(Code =(byte)ParameterCode.WorkshopId)]
        public byte workshop { get; set; }

        [DataMember(Code =(byte)ParameterCode.Level)]
        public int level { get; set; }

        [DataMember(Code = (byte)ParameterCode.Type)]
        public string type { get; set; }

        [DataMember(Code = (byte)ParameterCode.ServerId)]
        public string server { get; set; }
    }
}
