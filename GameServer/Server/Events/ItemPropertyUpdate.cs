using Common;
using Photon.SocketServer.Rpc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Server.Events {
    public class ItemPropertyUpdate {
        [DataMember(Code = (byte)ParameterCode.ItemId)]
        public string itemId { get; set; }

        [DataMember(Code = (byte)ParameterCode.ItemType)]
        public byte itemType { get; set; }

        [DataMember(Code = (byte)ParameterCode.Properties)]
        public Hashtable properties { get;  set; }
    }
}
