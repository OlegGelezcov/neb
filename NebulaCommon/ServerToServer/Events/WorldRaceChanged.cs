using Photon.SocketServer.Rpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NebulaCommon.ServerToServer.Events {
    public class WorldRaceChanged {
        [DataMember(Code =(byte)ServerToServerParameterCode.WorldId)]
        public string worldID { get; set; }

        [DataMember(Code =(byte)ServerToServerParameterCode.PreviousRace)]
        public byte previousRace { get; set; }

        [DataMember(Code =(byte)ServerToServerParameterCode.CurrentRace)]
        public byte currentRace { get; set; }
    }
}
