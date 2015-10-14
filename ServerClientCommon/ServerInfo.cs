using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace ServerClientCommon {
    public class ServerInfo : IInfo {

        public ServerType ServerType;
        public string IpAddress;
        public string Protocol;
        public int Port;
        public int Index;
        public string Application;
        public string[] Locations;

        public Hashtable GetInfo() {
            return new Hashtable {
                {(short)SPC.ServerType,     (byte)this.ServerType },
                {(short)SPC.IpAddress,      this.IpAddress  },
                {(short)SPC.Protocol,       this.Protocol   },
                {(short)SPC.Port,           this.Port       },
                {(short)SPC.Index,          this.Index      },
                {(short)SPC.Application,   this.Application},
                { (short)SPC.Locations, Locations }
            };
        }

        public string Key() {
            return string.Format("{0}-{1}", ServerType.ToString(), Index);
        }

        public override string ToString() {
            return string.Format("server type={0}, ip={1}, protocol={2}, port={3}, index={4}", ServerType, IpAddress, Protocol, Port, Index);
        }

        public void ParseInfo(Hashtable info) {
            this.ServerType = (ServerType)info.Value<byte>((short)SPC.ServerType, (byte)ServerClientCommon.ServerType.game);
            this.IpAddress = info.Value<string>((short)SPC.IpAddress);
            this.Protocol = info.Value<string>((short)SPC.Protocol);
            this.Port = info.Value<int>((short)SPC.Port);
            this.Index = info.Value<int>((short)SPC.Index);
            this.Application = info.Value<string>((short)SPC.Application);
            this.Locations = info.Value<string[]>((short)SPC.Locations);
        }

        public bool ContainsLocation(string worldID) {
            return Locations.Contains(worldID);
        }
    }
}
