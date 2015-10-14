using Common;
using Photon.SocketServer;
using Photon.SocketServer.Rpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelectCharacter.Operations {
    public class SetNewPriceOperationRequest : Operation{

        public SetNewPriceOperationRequest(IRpcProtocol protocol, OperationRequest request)
            : base(protocol, request) { }

        [DataMember(Code =(byte)ParameterCode.Login)]
        public string login { get; set; }

        [DataMember(Code =(byte)ParameterCode.GameRefId)]
        public string gameRefID { get; set; }

        [DataMember(Code =(byte)ParameterCode.CharacterId)]
        public string characterID { get; set; }

        [DataMember(Code =(byte)ParameterCode.ItemId)]
        public string storeItemID { get; set; }

        [DataMember(Code =(byte)ParameterCode.Price)]
        public int price { get; set; }
    }
}
