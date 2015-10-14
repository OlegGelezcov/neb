using Photon.SocketServer;
using Photon.SocketServer.Rpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace NebulaCommon.ServerToServer.Operations {
    public class UpdateCharacter : Operation{

        public UpdateCharacter() {

        }

        public UpdateCharacter(IRpcProtocol protocol, OperationRequest request)
            : base(protocol, request) {

        }

        [DataMember(Code =(byte)ServerToServerParameterCode.GameRefId, IsOptional =false)]
        public string GameRefId { get; set; }

        [DataMember(Code =(byte)ServerToServerParameterCode.CharacterId, IsOptional =false)]
        public string CharacterId { get; set; }

        [DataMember(Code = (byte)ServerToServerParameterCode.Workshop, IsOptional = false)]
        public byte Workshop { get; set; }


        [DataMember(Code = (byte)ServerToServerParameterCode.Race, IsOptional = false)]
        public byte Race { get; set; }

        [DataMember(Code = (byte)ServerToServerParameterCode.Alive, IsOptional = false)]
        public bool Deleted { get; set; }

        [DataMember(Code = (byte)ServerToServerParameterCode.Model, IsOptional = false)]
        public Hashtable Model { get; set; }

        [DataMember(Code =(byte)ServerToServerParameterCode.WorldId, IsOptional =true)]
        public string WorldId { get; set; }

        [DataMember(Code =(byte)ServerToServerParameterCode.Exp, IsOptional =true)]
        public int Exp { get; set; }
    }
}
