using Common;
using Photon.SocketServer;
using Photon.SocketServer.Rpc;
using System.Collections;

namespace SelectCharacter.Operations {
    public class WriteMailMessageOperationRequest : Operation {

        [DataMember(Code =(byte)ParameterCode.SourceLogin, IsOptional = false)]
        public string SourceLogin { get; set; }

        [DataMember(Code =(byte)ParameterCode.TargetLogin, IsOptional =false)]
        public string TargetLogin { get; set; }

        [DataMember(Code =(byte)ParameterCode.Title, IsOptional =false)]
        public string Title { get; set; }

        [DataMember(Code =(byte)ParameterCode.Body, IsOptional =false)]
        public string Body { get; set; }

        [DataMember(Code =(byte)ParameterCode.Attachments, IsOptional =true)]
        public Hashtable Attachments { get; set;}

        [DataMember(Code =(byte)ParameterCode.InventoryType)]
        public byte inventoryType { get; set; }

        public WriteMailMessageOperationRequest(IRpcProtocol protocol, OperationRequest operationRequest) : base(protocol, operationRequest) { }
    }
}
