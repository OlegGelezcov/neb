using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using System.Collections;
using ServerClientCommon;

namespace Nebula.Client
{
    public class ClientWorld : IInfo
    {
        public string Id { get; private set; }
        public Race OwnedRace { get; private set; }
        public ClientWorldZone Zone { get; private set; }

        public ClientWorld()
        {

        }

        public ClientWorld(Hashtable info )
        {
            this.ParseInfo(info);
        }

        public Hashtable GetInfo()
        {
            Hashtable info = new Hashtable();
            info.Add((int)SPC.Id, this.Id);
            info.Add((int)SPC.OwnedRace, this.OwnedRace.toByte());
            info.Add((int)SPC.ZoneInfo, (this.Zone != null) ? this.Zone.GetInfo() : new Hashtable());
            return info;
        }

        public void ParseInfo(Hashtable info)
        {
            this.Id = info.GetValue<string>((int)SPC.Id, string.Empty);
            this.OwnedRace = (Race)info.GetValue<byte>((int)SPC.OwnedRace, (byte)0);
            this.Zone = new ClientWorldZone(info.GetValue<Hashtable>((int)SPC.ZoneInfo, new Hashtable()));
        }
    }
}
