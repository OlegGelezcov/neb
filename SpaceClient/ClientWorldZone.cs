using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using System.Collections;
using ServerClientCommon;

namespace Nebula.Client
{
    public class ClientWorldZone : IInfo
    {
        public string Id { get; private set; }
        public string Name { get; private set; }
        public int Level { get; private set; }
        public Race InitiallyOwnedRace { get; private set; }

        public ClientWorldZone()
        {

        }
        public ClientWorldZone(Hashtable info )
        {
            this.ParseInfo(info);
        }

        public Hashtable GetInfo()
        {
            Hashtable info = new Hashtable();
            info.Add((int)SPC.Id, this.Id);
            info.Add((int)SPC.DisplayName, this.Name);
            info.Add((int)SPC.Level, this.Level);
            info.Add((int)SPC.InitialOwnedRace, this.InitiallyOwnedRace.toByte());
            return info;
        }

        public void ParseInfo(Hashtable info)
        {
            this.Id = info.GetValue<string>((int)SPC.Id, string.Empty);
            this.Name = info.GetValue<string>((int)SPC.DisplayName, string.Empty);
            this.Level = info.GetValue<int>((int)SPC.Level, 0);
            this.InitiallyOwnedRace = (Race)info.GetValue<byte>((int)SPC.InitialOwnedRace, (byte)0);
        }
    }
}
