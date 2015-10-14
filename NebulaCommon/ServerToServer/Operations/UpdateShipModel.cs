using Photon.SocketServer;
using Photon.SocketServer.Rpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NebulaCommon.ServerToServer.Operations {
    public class UpdateShipModel : Operation {
        public UpdateShipModel(IRpcProtocol rpcProtocol, OperationRequest operationRequest)
            : base(rpcProtocol, operationRequest) {

        }

        public UpdateShipModel() { }

        [DataMember(Code =(byte)ServerToServerParameterCode.GameRefId, IsOptional =false)]
        public string GameRefId { get; set; }

        [DataMember(Code =(byte)ServerToServerParameterCode.CharacterId, IsOptional =false)]
        public string CharacterId { get; set; }

        [DataMember(Code =(byte)ServerToServerParameterCode.SlotType, IsOptional =false)]
        public byte SlotType { get; set; }

        [DataMember(Code =(byte)ServerToServerParameterCode.TemplateId, IsOptional =false)]
        public string TemplateId { get; set; }

    }
}
