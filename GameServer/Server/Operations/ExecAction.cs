using Common;
using Photon.SocketServer;
using Photon.SocketServer.Rpc;
using System.Collections;
namespace Space.Server.Operations
{
    /*
    public class CompleteActionOperation : Operation
    {
        public CompleteActionOperation(IRpcProtocol protocol, OperationRequest request) : base(protocol, request) { }

        [DataMember(Code = (byte)ParameterCode.Action)]
        public string Action { get; set; }

        [DataMember(Code=(byte)ParameterCode.ItemId)]
        public string ItemId {get;set;}

        [DataMember(Code=(byte)ParameterCode.Parameters)]
        public object[] Parameters {get; set;}
    }*/

    public class ExecAction : Operation { 
        
        public ExecAction(IRpcProtocol protocol, OperationRequest request ) : base(protocol, request)
        {

        }

        [DataMember(Code = (byte)ParameterCode.Action)]
        public string Action { get; set; }

        [DataMember(Code = (byte)ParameterCode.ItemId)]
        public string ItemId { get; set; }

        [DataMember(Code = (byte)ParameterCode.Parameters)]
        public object[] Parameters { get; set; }
    }

    public class ExecActionResponse {

        [DataMember(Code = (byte)ParameterCode.Result)]
        public Hashtable Result { get; set; }

        [DataMember(Code = (byte)ParameterCode.Action)]
        public string Action { get; set; }

        [DataMember(Code = (byte)ParameterCode.ItemId)]
        public string ItemId { get; set; }
    }

}
