using NebulaCommon.SelectCharacter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Space.Game;
using MongoDB.Bson;
using Common;
using System.Collections;

namespace Space.Database {
    public class StatsDocument {

        public ObjectId Id { get; set; }

        public string CharacterId { get; set; }
        public int Exp { get; set; }
        public int Workshop { get; set; }
        public string Name { get; set; }
        public int Race { get; set; }

        public bool IsNewCharacter { get; set; }

        public Hashtable Model { get; set; }



        public void Set(PlayerCharacter sourceObject) {
            if(sourceObject == null ) {
                return;
            }
            this.Exp                = sourceObject.Exp;
            this.Workshop           =  sourceObject.Workshop;
            this.Name               = sourceObject.Name;
            this.Race               = (int)(byte)sourceObject.Race;
            this.CharacterId        = sourceObject.CharacterId;
            this.Model              = sourceObject.Model;
        }

        public PlayerCharacter SourceObject(IRes resource) {

            PlayerCharacter player = new PlayerCharacter();
            player.SetExp(Exp);
            player.SetWorkshop((Common.Workshop)Workshop);
            player.SetName(Name);
            player.SetRace((Common.Race)Race);
            player.SetCharacterId(CharacterId);
            player.SetModel(Model);
            return player;
        }
    }
}
