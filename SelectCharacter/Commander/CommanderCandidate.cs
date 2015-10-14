using Common;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using ServerClientCommon;

namespace SelectCharacter.Commander {
    public class CommanderCandidate : IInfoSource {
        public ObjectId Id { get; set; }

        public int race { get; set; }
        public string login { get; set; }
        public string gameRefID { get; set; }
        public string characterID { get; set; }
        public int voices { get; set; }
        public string guildName { get; set; }


        public void IncrementVoices() {
            voices++;
        }

        public Hashtable GetInfo() {
            return new Hashtable {
                { (int)SPC.Race, race },
                { (int)SPC.Login, login },
                { (int)SPC.GameRefId, gameRefID },
                { (int)SPC.CharacterId, characterID },
                { (int)SPC.Voices, voices },
                { (int)SPC.Guild, guildName }
            };
        }
    }
}
