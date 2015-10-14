using MongoDB.Bson;
using Nebula.Game.Components;
using Space.Game;
using Space.Game.Skills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Space.Database {
    public class SkillsDocument  {
        public ObjectId Id { get; set; }

        public string CharacterId { get; set; }

        public Dictionary<int, int> Skills { get; set; }

        public bool IsNewDocument { get; set; }

        public void Set(PlayerSkillsSave sourceObject) {

            Dictionary<int, int> skills = new Dictionary<int, int>();
            foreach (var pair in sourceObject.skills) {
                skills.Add(pair.Key, pair.Value);
            }
            this.Skills = skills;
            this.IsNewDocument = false;
        }

        public PlayerSkillsSave SourceObject(IRes resource) {
            return new PlayerSkillsSave { characterID = CharacterId, skills = Skills };
        }
    }

    public class PlayerSkillsSave {
        public Dictionary<int, int> skills;
        public string characterID;
    }
}
