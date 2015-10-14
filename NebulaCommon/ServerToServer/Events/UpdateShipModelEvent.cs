using Photon.SocketServer;
using Photon.SocketServer.Rpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NebulaCommon.ServerToServer.Events {
    public class UpdateShipModelEvent : DataContract {

        public UpdateShipModelEvent() { }

        public UpdateShipModelEvent(IRpcProtocol protocol, IEventData eventData )
            : base(protocol, eventData.Parameters) {

        }

        [DataMember(Code = (byte)ServerToServerParameterCode.GameRefId, IsOptional = false)]
        public string GameRefId { get; set; }

        [DataMember(Code = (byte)ServerToServerParameterCode.CharacterId, IsOptional = false)]
        public string CharacterId { get; set; }

        [DataMember(Code = (byte)ServerToServerParameterCode.SlotType, IsOptional = false)]
        public byte SlotType { get; set; }

        [DataMember(Code = (byte)ServerToServerParameterCode.TemplateId, IsOptional = false)]
        public string TemplateId { get; set; }
    }
}
