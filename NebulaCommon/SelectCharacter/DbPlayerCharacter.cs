using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Common;
using ServerClientCommon;

namespace NebulaCommon.SelectCharacter {

    public class DbPlayerCharacter : IInfoSource {

        public string CharacterId { get; set; }
        public string Name { get; set; }
        public int Workshop { get; set; }
        public int Race { get; set; }
        public bool Deleted { get; set; }
        public Hashtable Model { get;  set; }
        public string WorldId { get; set; }
        public int Exp { get; set; }
        public string guildID { get; set; }
        public int raceStatus { get; set; }

        public bool HasGuild() {
            return (false == string.IsNullOrEmpty(guildID));
        }

        public void SetModule(int type, string moduleId) {
            if(Model == null) {
                Model = new Hashtable();
            }
            Model[type] = moduleId;
        }

        public void SetModel(Hashtable source) {
            Model = new Hashtable();
            foreach(DictionaryEntry entry in source) {
                Model.Add((int)entry.Key, entry.Value);
            }
        }

        public void SetGuild(string guild) {
            guildID = guild;
        }

        public void SetName(string name) {
            this.Name = name;
        }


        public Hashtable GetInfo() {
            return new Hashtable {
                {(int)SPC.Id, CharacterId },
                {(int)SPC.Name, Name  },
                {(int)SPC.Workshop, Workshop },
                {(int)SPC.Race, Race },
                {(int)SPC.Deleted, Deleted },
                {(int)SPC.Model, Model},
                {(int)SPC.WorldId, WorldId },
                {(int)SPC.Exp, Exp },
                {(int)SPC.Guild, guildID },
                {(int)SPC.RaceStatus, raceStatus}
            };
        }
    }
}
