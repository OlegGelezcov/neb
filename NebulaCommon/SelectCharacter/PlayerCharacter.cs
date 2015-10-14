using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using ServerClientCommon;
using Space.Game.Resources;

namespace NebulaCommon.SelectCharacter {
    public class PlayerCharacter : IInfo {

        public string CharacterId { get;  set; }
        public string Name { get;  set; }
        public int Workshop { get;  set; }
        public int Exp { get;  set; }
        public int Race { get;  set; }
        public string Login { get; set; }

        public Hashtable Model { get; set; }

        public void AddExp(int add) {
            Exp += add;
        }

        public void SetExp(int exp) {
            Exp = exp;
        }

        public void SetWorkshop(Workshop w) {
            Workshop = (int)w;
        }

        public void SetName(string name) {
            Name = name;
        }

        public void SetRace(Race race) {
            Race = (int)race;
        }

        public void SetCharacterId(string characterId) {
            CharacterId = characterId;
        }

        public void SetModel(Hashtable model) {
            Model = model;
        }

        //public bool Alive { get; private set; }
        //public Dictionary<ShipModelSlotType, string> Model { get; private set; }

        public PlayerCharacter(Hashtable info) {
            this.ParseInfo(info);
        }

        public PlayerCharacter() { }

        public PlayerCharacter(string characterId) {
            CharacterId = characterId;
        }

        public Hashtable GetInfo() {
            var result = new Hashtable {
                {(int)SPC.Id,        this.CharacterId },
                {(int)SPC.Name,      this.Name  },
                {(int)SPC.Workshop,  this.Workshop },
                {(int)SPC.Race,      this.Race},
                {(int)SPC.Exp,       this.Exp},
                {(int)SPC.Model,     this.Model}
            };
            return result;
        }

        public void ParseInfo(Hashtable info) {
            this.CharacterId = info.Value<string>((int)SPC.Id, string.Empty);
            this.Name = info.Value<string>((int)SPC.Name, string.Empty);
            this.Workshop = info.Value<int>((int)SPC.Workshop, 0);
            this.Race = info.Value<int>((int)SPC.Race, 0);
            this.Exp = info.Value<int>((int)SPC.Exp, 0);
            this.Model = info.GetValue<Hashtable>((int)SPC.Model, new Hashtable());
        }
    }
}
