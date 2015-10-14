using NebulaCommon;
using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace NebulaCommon.ServerToServer.Operations {
    public class RegisterGameServer : Operation {
        
        public RegisterGameServer(IRpcProtocol rpcProtocol, OperationRequest operationRequest )
            : base(rpcProtocol, operationRequest) {

        }

        public RegisterGameServer() {

        }

        [DataMember(Code =(byte)ServerToServerParameterCode.GameServerAddress, IsOptional =false)]
        public string GameServerAddress { get; set; }

        [DataMember(Code =(byte)ServerToServerParameterCode.ServerId, IsOptional =false)]
        public string ServerId { get; set; }

        [DataMember(Code =(byte)ServerToServerParameterCode.TcpPort, IsOptional =true)]
        public int? TcpPort { get; set; }

        [DataMember(Code =(byte)ServerToServerParameterCode.UdpPort, IsOptional =true)]
        public int? UdpPort { get; set; }

        [DataMember(Code =(byte)ServerToServerParameterCode.WebSocketPort, IsOptional =true)]
        public int? WebSocketPort { get; set; }

        [DataMember(Code =(byte)ServerToServerParameterCode.ServerState, IsOptional =true)]
        public int ServerState { get; set; }

        [DataMember(Code =(byte)ServerToServerParameterCode.ServerType, IsOptional =false)]
        public byte ServerType { get; set; }

    }
}
