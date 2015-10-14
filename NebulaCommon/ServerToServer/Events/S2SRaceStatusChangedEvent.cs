using Photon.SocketServer.Rpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NebulaCommon.ServerToServer.Events {
    public class S2SRaceStatusChangedEvent {

        [DataMember(Code =(byte)ServerToServerParameterCode.RaceStatus)]
        public int raceStatus { get; set; }

        [DataMember(Code =(byte)ServerToServerParameterCode.GameRefId)]
        public string gameRefID { get; set; }

        [DataMember(Code =(byte)ServerToServerParameterCode.CharacterId)]
        public string characterID { get; set; }
    }
}
